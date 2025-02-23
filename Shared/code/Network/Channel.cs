using Godot;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SkillQuest.Network;

public class Channel {
    public string Name { get; }
    
    public Multiplayer Multiplayer { get; }
    
    // TODO: public bool Encrypt { get; set; }

    
    const bool DEBUG = true;
    
    public Channel(Multiplayer networker, string name){
        Multiplayer = networker;
        Name = name;
    }

    public async Task Send(Connection.Client? connection, Network.Packet packet, bool encrypt = true){
        packet.Channel = Name;
        if ( DEBUG ) GD.Print( $"{Name} -> {packet.GetType().Name}");

        try {
            await connection?.Send(packet, encrypt);
        } catch (Exception e) {
            GD.Print( $"Unable to send packet {packet.GetType().Name} {e}");

            if (connection.Status == Connection.State.Disconnecting) {
                connection.Disconnect();
            }
        }
    }

    public async Task Receive(Connection.Client connection, Network.Packet packet){
        if ( DEBUG )  GD.Print( $"{Name} <- {packet.GetType().Name}");
        if (_handlers.TryGetValue(packet.GetType(), out var handler)) {
            handler.Invoke(connection, packet);
        } else {
            GD.Print( $"No handler for {packet.GetType().Name}");
        }
    }

    ConcurrentDictionary<Type, Action<Connection.Client, Network.Packet>> _handlers = new();

    public delegate void DoPacket< TPacket >( Connection.Client connection, TPacket packet ) where TPacket : Packet;
    
    public void Subscribe<TPacket>(Channel.DoPacket<TPacket> handler) where TPacket : Network.Packet{
        Multiplayer.AddPacket< TPacket >();
        _handlers[ typeof(TPacket) ] = ( clientConnection, packet ) => handler( clientConnection, packet as TPacket ?? throw new ArgumentNullException( nameof(packet) ) );
    }

    public void Unsubscribe<TPacket>() where TPacket : Network.Packet{
        _handlers.TryRemove( typeof(TPacket), out _);
    }

    public void Reset(){
        _handlers.Clear();
    }
}