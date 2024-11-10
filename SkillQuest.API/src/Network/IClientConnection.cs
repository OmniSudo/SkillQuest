using System.Net;
using System.Security.Cryptography;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.API.Network;

public interface IClientConnection : IConnection {
    public byte[] Key { get; set; }
    
    public void Send(IPacket packet, bool udp = false );

    /// <summary>
    /// Interrupt the timeout
    /// </summary>
    void InterruptTimeout();

    void Disconnect();

    public delegate void DoConnect(IClientConnection connection);
    
    public event DoConnect Connected;

    public delegate void DoDisconnect(IClientConnection connection);
    
    public event DoDisconnect Disconnected;

    void Connect(IPEndPoint endpoint);
}
