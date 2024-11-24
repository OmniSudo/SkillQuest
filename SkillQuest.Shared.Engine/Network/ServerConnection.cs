using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Network.Packet;

namespace SkillQuest.Shared.Engine.Network;

internal class ServerConnection : IServerConnection{

    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public void Stop(){
        
    }

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();

    public ServerConnection(Networker networker, short port){
        Networker = networker;
        EndPoint = new IPEndPoint(IPAddress.Any, port);
    }

    public TcpListener Server { get; private set; }

    public RSA RSA { get; } = new RSACryptoServiceProvider();

    public async Task Listen(){
        Server = new TcpListener( EndPoint );
        Server.Start();
        Accept();
    }

    public bool Running { get; set; }
    
    private async Task Accept(){
        Running = true;
        Console.WriteLine($"Listening @ {EndPoint}");
        while ( Running ) {
            var client = Server.AcceptTcpClient();
            Console.WriteLine($"Accepted @ {client.Client.RemoteEndPoint}");
            var connection = _clients[client.Client.RemoteEndPoint as IPEndPoint] = new LocalConnection(this, client);
            
            connection.Receive();
            Networker.CreateChannel( new("packet://skill.quest/system") ).Send( connection, new RSAPacket() { PublicKey = RSA.ExportRSAPublicKeyPem() } );
        }
    }
    
    protected internal async Task OnConnected( IClientConnection connection ){
        Connected?.Invoke(this, connection);
        Console.WriteLine($"Connected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoConnected? Connected;
    
    protected internal async Task OnDisconnected( IClientConnection connection ){
        Disconnected?.Invoke(this, connection);
        Console.WriteLine($"Disconnected @ {connection.EndPoint}");
    }

    public event IServerConnection.DoDisconnected? Disconnected;

    public event IServerConnection.DoDeafen? Deafen;

    public void Disconnect(IClientConnection connection){
        _clients.TryRemove(connection.EndPoint, out _);

        Disconnected?.Invoke(this, connection);
        Console.WriteLine($"Disconnected @ {connection.EndPoint}");
    }

    public async Task Receive(IClientConnection connection, API.Network.Packet packet){
        try {
            connection.Receive(packet);
        } catch (Exception e) {
            Console.WriteLine( $"Unable to handle {packet.Channel} {packet.GetType().Name}:\n{e}" );
        }
    }
}
