namespace SkillQuest.Shared.Game.Addons.SkillQuestMP.Shared.Packet.Credentials;

public class LoginAuthenticationStatusPacket : API.Network.Packet{
    public bool Success { get; set; }

    public string? Reason { get; set; }
}
