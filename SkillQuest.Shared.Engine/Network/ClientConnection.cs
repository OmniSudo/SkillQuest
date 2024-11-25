using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

public class ClientConnection : IClientConnection{
    protected TcpClient? Connection { get; set; } = null;
    
    protected NetworkStream _stream;


    public string EMail { get; set; }

    public Guid Id { get; set; }

    public string AuthToken { get; set; }

    public Guid Session { get; set; }
    
    public virtual RSA RSA { get; } = new RSACryptoServiceProvider();

    public virtual Aes AES { get; set; }

    public bool Running { get; set; }

    public async Task Send(API.Network.Packet packet, bool encrypt = true){
        var serialized = JsonSerializer.Serialize(packet, packet.GetType());

        var typename = packet.GetType().FullName;

        Console.WriteLine(typename + $": {serialized}");

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
        throw new NotImplementedException();
    }

    public void Disconnect(){
        Console.WriteLine($"Disconnecting @ {EndPoint}");
        Disconnected?.Invoke(this);
        Console.WriteLine($"Disconnected @ {EndPoint}");
        _stream.Close();
        _ = _stream.DisposeAsync();
        Console.WriteLine($"Disposed @ {EndPoint}");
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    public async Task Listen(){
        try {
            while ( IsOpen ) {
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

                if (Networker.Packets.TryGetValue(split[0], out var type)) {
                    API.Network.Packet? packet = JsonSerializer.Deserialize(split[1], type) as API.Network.Packet;

                    await Receive(packet);
                }
            }
        } catch (Exception e) {
            Console.WriteLine($"Packet Exception:\n{e}");
        }
    }

    public bool IsOpen => _stream?.Socket?.Connected??false;
    
    public async Task Receive(API.Network.Packet packet){
        if (packet?.Channel is null || !Networker.Channels.TryGetValue(packet.Channel, out var channel)) {
            Console.WriteLine("Null Channel: " + packet.GetType().Name);
            return;
        }
        await channel?.Receive(this, packet);
    }


    public INetworker Networker { get; }

    public virtual IPEndPoint EndPoint { get; }
}
