using System.Text.Json.Serialization;
using SkillQuest.API.Network;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Shared.Game;

public class TestPacket : Packet{
    public string Message { get; set; }
}
