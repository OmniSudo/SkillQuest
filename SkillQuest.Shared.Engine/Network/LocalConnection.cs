using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SkillQuest.API.Network;
using Timer = System.Timers.Timer;

namespace SkillQuest.Shared.Engine.Network;

using static State;

internal class LocalConnection : ILocalConnection{
    public LocalConnection(
        ServerConnection server,
        TcpClient tcpconnection
    ){
        Server = server;
        Connection = tcpconnection;
        _timeout = new Timer(TimeSpan.FromSeconds(10));
        _stream = Connection.GetStream();

        _timeout.Elapsed += (sender, args) => {
            Server.Disconnect(this);
            _timeout.Enabled = false;
        };
    }

    NetworkStream _stream;

    public TcpClient Connection { get; set; }

    public IServerConnection Server { get; }

    public INetworker Networker => Server?.Networker ?? SH.Net;

    public IPEndPoint? EndPoint => Connection.Client.RemoteEndPoint as IPEndPoint;

    public string EMail { get; set; }

    public Guid Id { get; set; }

    public string AuthToken { get; set; }

    public Guid Session { get; set; }

    public byte[] Key { get; set; }

    Timer _timeout;

    public RSA RSA => Server.RSA;

    public Aes AES { get; set; }

    public void Send(Packet packet, bool encrypt = true){
        var serialized = JsonSerializer.Serialize(packet, packet.GetType());

        var typename = packet.GetType().FullName;

        var data = typename + (char)0x0 + serialized;
        byte[] ciphertext;

        if (encrypt) {
            ICryptoTransform encryptor = AES.CreateEncryptor(AES.Key, AES.IV);

            using (var msEncrypt = new MemoryStream()) {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                }
                ciphertext = msEncrypt.ToArray();
            }
        } else {
            ciphertext = Encoding.UTF8.GetBytes(data);
        }
        byte[] len = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ciphertext.Length));

        _stream.Write([encrypt ? (byte)0xFF : (byte)0x00], 0, 1);
        _stream.Write(len, 0, len.Length);
        _stream.Write(ciphertext, 0, ciphertext.Length);
    }

    public void InterruptTimeout(){
        _timeout.Enabled = false;
    }

    public void Disconnect(){
        Disconnected?.Invoke(this);
        Connection.Close();
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    public void Connect(IPEndPoint endpoint){
        throw new InvalidOperationException();
    }

    public async Task Receive(){
        while ( Server.Running ) {
            try {
                byte[] enc = new byte[1];
                var lengthRead = await _stream.ReadAsync(enc, 0, enc.Length);

                var len = new byte[sizeof(int)];

                if (await _stream.ReadAsync(len, 0, len.Length) != len.Length) {
                    // TODO: Send disconnect message
                    _stream.Close();
                    return;
                }
                ;
                var data = new byte[IPAddress.NetworkToHostOrder(BitConverter.ToInt32(len, 0))];

                if (await _stream.ReadAsync(data, 0, data.Length) != data.Length) {
                    // TODO: Send disconnect message
                    _stream.Close();
                    return;
                }

                string plaintext = "{}";

                if (len[0] != 0x00) {
                    ICryptoTransform decryptor = AES.CreateDecryptor(AES.Key, AES.IV);
                    byte[] decryptedBytes;

                    using (var msDecrypt = new MemoryStream(data)) {
                        using (var csDecrypt =
                               new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                            using (var msPlain = new MemoryStream()) {
                                csDecrypt.CopyTo(msPlain);
                                decryptedBytes = msPlain.ToArray();
                            }
                        }
                    }
                    plaintext = Encoding.UTF8.GetString(decryptedBytes);
                } else {
                    plaintext = Encoding.UTF8.GetString(data);
                }
                var split = plaintext.Split((char)0x0);
                var type = Type.GetType(split[0]);
                Packet? packet = JsonSerializer.Deserialize(split[1], type) as Packet;

                Receive(packet);
            } catch (Exception e) {
                Console.WriteLine($"Packet Exception:\n{e}");
            }
        }
    }

    public void Receive(Packet packet){
        Networker.Channels.TryGetValue(packet.Channel, out var channel);
        channel?.Receive(this, packet);
    }
}
