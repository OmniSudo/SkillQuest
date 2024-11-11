using System.Collections.Immutable;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace SkillQuest.API.Network;

public interface IServerConnection : IConnection{
    public void Broadcast(Packet packet);
    public void Send(IPEndPoint endpoint, Packet packet);
    
    public delegate void DoConnected(IServerConnection server, IClientConnection client);

    public event DoConnected Connected;

    public delegate void DoDisconnected(IServerConnection server, IPEndPoint endpoint);

    public event DoDisconnected Disconnected;

    public delegate void DoDeafen( IServerConnection connection );
    
    public event DoDeafen Deafen;

    void Disconnect(IClientConnection connection);

    void Stop();
    
    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients { get; }
}
