using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Network;

public class Channel : IChannel {

    public string Name { get; }

    public bool Encrypt { get; set; }

    public void Send<TPacket>(IConnection connection, TPacket packet) where TPacket : IPacket{
        throw new NotImplementedException();
    }

    public void Send(IConnection connection, IPacket packet){
        throw new NotImplementedException();
    }

    public void Add(Action<IConnection, IPacket> handler){
        throw new NotImplementedException();
    }

    public void Drop(Action<IConnection, IPacket> handler){
        throw new NotImplementedException();
    }

    public void Reset(){
        throw new NotImplementedException();
    }
}
