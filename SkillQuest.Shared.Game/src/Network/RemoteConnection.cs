using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lidgren.Network;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

public class RemoteConnection : IRemoteConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public NetClient Client { get; set; } = null;

    RSA RSA { get; } = new RSACryptoServiceProvider();

    NetAESEncryption AES { get; set; }

    public RemoteConnection(INetworker networker, IPEndPoint endpoint){
        Networker = networker;
        EndPoint = endpoint;
        Key = new byte[16];
        new Random().NextBytes( Key );
        
        var config = new NetPeerConfiguration("SkillQuest");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
        Client = new NetClient(config);
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        Client.RegisterReceivedCallback(new SendOrPostCallback(Received));

        Client.Start();
        Client.DiscoverKnownPeer(EndPoint);
    }

    public byte[] Key { get; set; }

    public void Send(IPacket packet, bool udp = false){
        var serialized = JsonSerializer.Serialize(packet);

        NetOutgoingMessage message = Client.CreateMessage();
        message.Write(packet.GetType().FullName);

        message.Write(serialized);

        Client.SendMessage(message, udp ? NetDeliveryMethod.Unreliable : NetDeliveryMethod.ReliableOrdered, 0);
    }

    public void InterruptTimeout(){
        AES = new NetAESEncryption(Client);
        AES.SetKey(Key, 0, Key.Length);
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
                    // TODO: message.Decrypt();
                    var typename = message.ReadString();
                    var data = message.ReadString();
                    var type = Type.GetType(typename);

                    IPacket? packet = JsonSerializer.Deserialize(data, type) as IPacket;

                    if (packet is not null) Receive(packet);
                    else Console.WriteLine("Unknown packet type {0}", typename); // TODO: Log ERROR
                    break;
                default:
                    Console.WriteLine("Unhandled Message Type: {0}", message.MessageType);
                    break;
            }
        }
    }

    void Receive(IPacket packet){
        Console.WriteLine(JsonSerializer.Serialize(packet));
    }
}
