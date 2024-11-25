using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Network.Packet;

namespace SkillQuest.Shared.Engine.Network;

class ServerConnection : IServerConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public void Stop(){
        Running = false;
    }

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();

    public ServerConnection(Networker networker, short port){
        Networker = networker;
        EndPoint = new IPEndPoint(IPAddress.Any, port);
    }

    public TcpListener Server { get; private set; }

    public RSA RSA { get; } = new RSACryptoServiceProvider();

    Thread _thread;
    
    public void Listen(){
        _thread = new Thread(() => {
            Server = new TcpListener(EndPoint);
            Server.Start();
            Console.WriteLine($"Listening @ {EndPoint}");

            Running = true;
            while ( Running ) {
                Server.BeginAcceptTcpClient(ar => {
                    var client = Server.EndAcceptTcpClient(ar );
                    Console.WriteLine($"Accepted @ {client.Client.RemoteEndPoint}");

                    var connection = _clients[client.Client.RemoteEndPoint as IPEndPoint] =
                        new LocalClientConnection(this, client);

                    connection.Listen();

                    Networker.SystemChannel.Send(connection,
                        new RSAPacket() { PublicKey = RSA.ExportRSAPublicKeyPem() });
                }, null);
            }
        });
        _thread.Start();
    }

    public bool Running { get; set; }
    
    protected internal void OnConnected(IClientConnection connection){
        Connected?.Invoke(this, connection);
        Console.WriteLine($"Connected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoConnected? Connected;

    protected internal void OnDisconnected(IClientConnection connection){
        Disconnected?.Invoke(this, connection);
        Console.WriteLine($"Disconnected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoDisconnected? Disconnected;

    public event IServerConnection.DoDeafen? Deafen;

    public void Disconnect(IClientConnection connection){
        _clients.TryRemove(connection.EndPoint, out var client);
        
        Disconnected?.Invoke(this, connection);
        client.Disconnect();
    }
}
