namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;
using Doohickey = Shared.Game.ECS.Doohickey;

public class CharacterCreator : Doohickey {
    public override Uri? Uri { get; } = new Uri( "cl://control.skill.quest/character/create" );

}
