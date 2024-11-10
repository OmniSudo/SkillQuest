using System.Text.Json.Serialization;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Shared.Game;

public class TestPacket : IPacket{
    public string Message { get; set; } = "TestPacket";
}
