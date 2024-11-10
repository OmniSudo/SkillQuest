using System.Collections.Immutable;
using System.Net;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

public interface INetworker{
    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients { get; }
    
    public ImmutableDictionary<IPEndPoint, IServerConnection> Servers { get; }
    
    public ImmutableDictionary< string, IChannel > Channels { get; }


    public Task<IClientConnection?> Connect(IPEndPoint endpoint);

    public void Listen(ILocalConnection connection);

    public Task<IServerConnection?> Host(short port);
}
