using System.Text.Json.Nodes;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.API.Network;

/// <summary>
/// A connection located on the server
/// </summary>
public interface ILocalConnection: IClientConnection{
    public IServerConnection Server { get; }
}
