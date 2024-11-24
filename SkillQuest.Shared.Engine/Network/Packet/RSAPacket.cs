using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

public class RSAPacket : Packet {
    public string PublicKey { get; set; }
}
