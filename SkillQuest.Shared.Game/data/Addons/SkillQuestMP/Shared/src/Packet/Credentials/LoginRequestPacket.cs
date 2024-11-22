namespace SkillQuest.Shared.Game.Addons.SkillQuestMP.Shared.Packet.Credentials;

public class LoginRequestPacket : API.Network.Packet{
    public string Email { get; set; }
    
    public string AuthToken { get; set; }
}
