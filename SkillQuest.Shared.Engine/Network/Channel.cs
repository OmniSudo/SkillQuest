using System.Collections.Concurrent;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

internal class Channel : IChannel {
    public string Name { get; init; }

    const bool DEBUG = false;

    public void Send(IClientConnection? connection, API.Network.Packet packet){
        packet.Channel = Name;
        connection?.Send(packet);
        if ( DEBUG ) Console.WriteLine( $"{Name} -> {packet.GetType().Name}");
    }

    public void Receive(IClientConnection connection, API.Network.Packet packet){
        if ( DEBUG ) Console.WriteLine( $"{Name} <- {packet.GetType().Name}");
        if (_handlers.TryGetValue(packet.GetType(), out var handler)) {
            handler.Invoke(connection, packet);
        } else {
            Console.WriteLine( $"No handler for {packet.GetType().Name}");
        }
    }

    ConcurrentDictionary<Type, Action<IClientConnection, API.Network.Packet>> _handlers = new();

    public void Subscribe<TPacket>(IChannel.DoPacket<TPacket> handler) where TPacket : API.Network.Packet{
        _handlers[ typeof(TPacket) ] = ( clientConnection, packet ) => handler( clientConnection, packet as TPacket ?? throw new ArgumentNullException( nameof(packet) ) );
    }

    public void Unsubscribe<TPacket>() where TPacket : API.Network.Packet{
        _handlers.TryRemove( typeof(TPacket), out _);
    }

    public void Reset(){
        _handlers.Clear();
    }
}
