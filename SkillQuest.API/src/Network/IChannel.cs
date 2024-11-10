using SkillQuest.Shared.Game.Network;

namespace SkillQuest.API.Network;

public interface IChannel{
    public string Name { get; }

    public bool Encrypt { get; set; }
    
    public void Send < TPacket > (IConnection connection, TPacket packet) where TPacket : IPacket;
    
    public void Send ( IConnection connection, IPacket packet );
    
    public void Add ( Action< IConnection, IPacket > handler );
    
    public void Drop ( Action< IConnection, IPacket > handler );

    public void Reset();
}
