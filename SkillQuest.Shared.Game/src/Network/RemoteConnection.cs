using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lidgren.Network;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

internal class RemoteConnection : IRemoteConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public NetClient Client { get; set; } = null;

    RSA RSA { get; } = new RSACryptoServiceProvider();

    internal NetEncryption Encryption { get; set; }

    public RemoteConnection(INetworker networker, IPEndPoint endpoint){
        Networker = networker;
        EndPoint = endpoint;
        Key = new byte[16];
        new Random().NextBytes(Key);

        var config = new NetPeerConfiguration("SkillQuest");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
        Client = new NetClient(config);
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        Client.RegisterReceivedCallback(new SendOrPostCallback(Received));

        Client.Start();
        Client.DiscoverKnownPeer(EndPoint);
    }

    public string EMail { get; set; }

    public Guid Id { get; set; }

    public string AuthToken { get; set; }

    public Guid Session { get; set; }

    public byte[] Key { get; set; }

    public void Send(Packet packet, bool udp = false){
        var serialized = JsonSerializer.Serialize(packet, packet.GetType());

        NetOutgoingMessage message = Client.CreateMessage();
        var typename = packet.GetType().FullName;

        var data = typename + (char)0x0 + serialized;
        message.Write(data);

        if (!message.Encrypt(Encryption)) return;

        Client.SendMessage(
            message,
            udp ? NetDeliveryMethod.Unreliable : NetDeliveryMethod.ReliableOrdered,
            0
        );
    }

    public void InterruptTimeout(){
        Encryption = new NetXtea(Client, Key);
    }

    public void Disconnect(){
        if (Client.ConnectionStatus == NetConnectionStatus.Connected) {
            Disconnected?.Invoke(this);
        }
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    public void Connect(IPEndPoint endpoint){
        Client?.Connect(endpoint);
    }

    void Received(object? peer){
        NetIncomingMessage message;

        while ( (message = Client.ReadMessage()) != null ) {
            switch (message.MessageType) {
                case NetIncomingMessageType.DiscoveryResponse:
                    var publicKey = Convert.FromBase64String(message.ReadString());

                    if (publicKey.Length > 0) {
                        RSA.Create();
                        RSA.ImportRSAPublicKey(publicKey, out _);

                        var response = Client.CreateMessage();
                        response.Write(Convert.ToBase64String(RSA.Encrypt(Key, RSAEncryptionPadding.Pkcs1)));

                        Client.SendDiscoveryResponse(
                            response,
                            message.SenderEndPoint
                        );
                    } else {
                        Connect(message.SenderEndPoint);
                    }
                    break;
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine(text); // TODO: Proper logger
                    break;
                case NetIncomingMessageType.StatusChanged:
                    var status = message.SenderConnection.Status;

                    switch (status) {
                        case NetConnectionStatus.Connected:
                            InterruptTimeout();
                            Connected?.Invoke(this);
                            break;
                        case NetConnectionStatus.Disconnected:
                            Disconnected?.Invoke(this);
                            break;
                        default:
                            break;
                    }
                    break;
                case NetIncomingMessageType.Data:
                    if (!message.Decrypt(Encryption)) break;

                    string typename = "";

                    try {
                        var data = message.ReadString();
                        var split = data.Split((char)0x0);
                        var type = Type.GetType(split[0]);

                        Packet? packet = JsonSerializer.Deserialize(split[1], type) as Packet;

                        if (packet is not null) {
                            Receive(packet);
                            break;
                        }
                        Console.WriteLine("Unknown packet type {0}", split[0]); // TODO: Log ERROR
                    } catch (Exception e) {
                        Console.WriteLine($"Packet Exception:\n{e}");
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled Message Type: {0}", message.MessageType);
                    break;
            }
        }
    }

    public void Receive(Packet packet){
        Networker.Channels.TryGetValue(packet.Channel, out var channel);
        channel?.Receive(this, packet);
    }
}
