using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lidgren.Network;
using SkillQuest.API.Network;
using Timer = System.Timers.Timer;

namespace SkillQuest.Shared.Game.Network;

using static State;

public class ServerConnection : IServerConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint => new IPEndPoint(IPAddress.Any, Port);

    public short Port { get; }

    public void Stop(){
        Server.Shutdown( "STOPPING SERVER NET" );
    }

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();

    public ServerConnection(Networker networker, short port){
        Networker = networker;
        Port = port;
    }

    public NetServer Server { get; set; }

    RSA RSA { get; } = new RSACryptoServiceProvider();

    public async Task Listen(){
        NetPeerConfiguration config = new NetPeerConfiguration("SkillQuest");
        config.MaximumConnections = 25; // TODO: Change max connections
        config.LocalAddress = EndPoint.Address;
        config.Port = EndPoint.Port;
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        RSA.Create();

        Server = new NetServer(config);
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        Server.RegisterReceivedCallback(new SendOrPostCallback(Received));
        Server.Start();
    }

    void Received(object? state){
        var message = Server.ReadMessage();

        if (message is not null) {
            switch (message.MessageType) {
                case NetIncomingMessageType.DiscoveryRequest:
                    var response = Server.CreateMessage();
                    response.Write(Convert.ToBase64String( RSA.ExportRSAPublicKey()));

                    Server.SendDiscoveryResponse(
                        response, message.SenderEndPoint
                    );
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    var key64 = message.ReadString();
                    var key = RSA.Decrypt(Convert.FromBase64String(key64), RSAEncryptionPadding.Pkcs1);

                    var client = _clients[message.SenderEndPoint] =
                        new LocalConnection(
                            this,
                            message.SenderEndPoint
                        ) {
                            Key = key,
                        };
                    
                    Console.WriteLine( $"Received Key from {client.EndPoint}");

                    var connect = Server.CreateMessage();
                    connect.Write("");

                    Server.SendDiscoveryResponse(
                        connect, message.SenderEndPoint
                    );
                    
                    break;
                case NetIncomingMessageType.StatusChanged:
                    var status = message.SenderConnection.Status;

                    switch (status) {
                        case NetConnectionStatus.Connected:
                            
                            var connected = _clients[message.SenderEndPoint];

                            if (connected is not null) {
                                Console.WriteLine($"Connected @ {message.SenderEndPoint}");

                                (connected as LocalConnection)?.Connect(message.SenderConnection);
                                connected.InterruptTimeout();
                                Connected?.Invoke(this, connected);
                            }
                            break;
                        case NetConnectionStatus.Disconnected:
                            Disconnect( _clients[ message.SenderEndPoint ] );
                            break;
                        default:
                            break;
                    }
                    break;
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine(text); // TODO: Proper logger
                    break;
                case NetIncomingMessageType.Data:
                    var connection = this._clients[message.SenderEndPoint];

                    if (!message.Decrypt((connection as LocalConnection)?.Encryption)) break;
                    
                    var length = message.ReadInt32();
                    message.ReadBytes(length, out var bytes);
                    var typename = Encoding.UTF8.GetString(bytes);
                    length = message.ReadInt32();
                    message.ReadBytes(length, out bytes);
                    var data = Encoding.UTF8.GetString(bytes);
                    var type = Type.GetType(typename);

                    if (type is not null) {
                        try {
                            Packet? packet = JsonSerializer.Deserialize(data, type) as Packet;

                            if (packet is not null) {
                                Receive(connection, packet);
                                break;
                            }
                            Console.WriteLine("Unknown packet type {0}", typename); // TODO: Log ERROR
                        } catch (Exception e) {
                            Console.WriteLine($"Packet Exception:\n{e}"); // TODO: Log ERROR
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled Message Type: {0}", message.MessageType);
                    break;
            }
        }
    }

    /// <summary>
    /// Broadcasts to all clients
    /// </summary>
    /// <param name="jsonObject"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Broadcast(Packet packet){
        foreach (var client in Clients) {
            Send(client.Value.EndPoint, packet);
        }
    }

    public void Send(IPEndPoint endpoint, Packet packet){
        _clients.TryGetValue( endpoint, out var client );
        client?.Send(packet);
    }

    public event IServerConnection.DoConnected? Connected;

    public event IServerConnection.DoDisconnected? Disconnected;

    public event IServerConnection.DoDeafen? Deafen;

    public void Disconnect(IClientConnection connection){
        _clients.TryRemove(connection.EndPoint, out _);

        Console.WriteLine($"Disconnected @ {connection.EndPoint}");
        Disconnected?.Invoke(this, connection);
    }

    public async Task Receive(IClientConnection connection, Packet packet){
        try {
            connection.Receive(packet);
        } catch (Exception e) {
            Console.WriteLine( $"Unable to handle {packet.Channel} {packet.GetType().Name}:\n{e}" );
        }
    }
}
