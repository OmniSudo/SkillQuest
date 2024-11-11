namespace SkillQuest.API.Network;

public interface IChannel{
    public string Name { get; }

    // TODO: public bool Encrypt { get; set; }
    
    public void Send ( IClientConnection? connection, Packet packet );

    public void Receive(IClientConnection connection, Packet packet);

    public delegate void DoPacket< TPacket >( IClientConnection connection, TPacket packet ) where TPacket : Packet;

    public void Subscribe < TPacket >( DoPacket<TPacket> handler ) where TPacket : Packet;

    public void Unsubscribe< TPacket >() where TPacket : Packet;

    public void Reset();
}
