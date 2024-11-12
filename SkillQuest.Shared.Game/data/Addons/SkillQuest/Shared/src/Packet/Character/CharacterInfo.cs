namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

public class CharacterInfo(Guid userId, Guid characterId, string name, Uri world, Uri uri){
    public Guid UserId { get; set; } = userId;

    public Guid CharacterId { get; set; } = characterId;

    public string Name { get; set; } = name;

    public Uri Uri { get; set; } = uri;

    public Uri World { get; set; } = world;
}
