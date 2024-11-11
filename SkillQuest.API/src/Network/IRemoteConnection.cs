using System.Net;
using System.Text.Json.Nodes;

namespace SkillQuest.API.Network;

/// <summary>
/// A connection located on the client
/// </summary>
public interface IRemoteConnection: IClientConnection {
    public void Send(Packet packet, bool udp = false );
    
    public void Connect(IPEndPoint endpoint);
}
