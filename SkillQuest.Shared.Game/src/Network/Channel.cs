using System.Collections.Concurrent;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

internal class Channel : IChannel {
    public string Name { get; init; }

    public void Send(IClientConnection? connection, Packet packet){
        packet.Channel = Name;
        connection?.Send(packet);
    }

    public void Receive(IClientConnection connection, Packet packet){
        if (_handlers.TryGetValue(packet.GetType(), out var handler)) {
            handler.Invoke(connection, packet);
        }
    }

    ConcurrentDictionary<Type, Action<IClientConnection, Packet>> _handlers = new();

    public void Subscribe<TPacket>(IChannel.DoPacket<TPacket> handler) where TPacket : Packet{
        _handlers[ typeof(TPacket) ] = ( clientConnection, packet ) => handler( clientConnection, packet as TPacket ?? throw new ArgumentNullException( nameof(packet) ) );
    }

    public void Unsubscribe<TPacket>() where TPacket : Packet{
        _handlers.TryRemove( typeof(TPacket), out _);
    }

    public void Reset(){
        _handlers.Clear();
    }
}
