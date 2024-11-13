using SkillQuest.API.Network;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

public class CharacterCreatorCreationRequestPacket : Packet{
    public CharacterInfo Character { get; set; }
}
