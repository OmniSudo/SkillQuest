using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

public sealed class Networker : INetworker {

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    public ImmutableDictionary<IPEndPoint, IServerConnection> Servers => _servers.ToImmutableDictionary();

    public ImmutableDictionary<string, IChannel> Channels => _channels.ToImmutableDictionary();

    private ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();
    private ConcurrentDictionary<IPEndPoint, IServerConnection> _servers = new();
    private ConcurrentDictionary<string, IChannel> _channels = new();

    public Task< IClientConnection? > Connect(IPEndPoint endpoint){
        var client = new RemoteConnection( this, endpoint );
        _clients.TryAdd(endpoint, client);
        
        TaskCompletionSource<IClientConnection> tcs = new();

        client.Connected += (connection) => {
            tcs.SetResult(connection);
        };
            
        client.Disconnected += (connection) => {
            Console.WriteLine( $"Droppped {connection.EndPoint}" );
            _clients.TryRemove(connection.EndPoint, out _);
        };
        
        return tcs.Task;
    }

    public void Listen(ILocalConnection connection){
        _clients[connection.EndPoint] = connection;
    }

    public async Task< IServerConnection? > Host(short port){
        var server = new ServerConnection( this, port );
        await server.Listen();
        return server;
    }
}
