namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Select;

public class CharacterJoinedWorldPacket : API.Network.Packet {
    public Guid CharacterId { get; set; }

    public string World { get; set; } = "";
}