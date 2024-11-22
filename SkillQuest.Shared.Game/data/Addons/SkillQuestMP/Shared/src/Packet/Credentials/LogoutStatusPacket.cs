namespace SkillQuest.Shared.Game.Addons.SkillQuestMP.Shared.Packet.Credentials;

public class LogoutStatusPacket : API.Network.Packet {
    public bool Success { get; set; }
}
