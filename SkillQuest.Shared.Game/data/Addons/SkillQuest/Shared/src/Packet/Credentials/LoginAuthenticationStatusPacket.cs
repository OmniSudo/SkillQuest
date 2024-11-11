using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Credentials;

public class LoginAuthenticationStatusPacket : API.Network.Packet{
    public bool Success { get; set; }

    public string? Reason { get; set; }
}
