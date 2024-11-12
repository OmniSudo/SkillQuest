using SkillQuest.API.ECS;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.CharacterSelect;

using Doohickey = Shared.Game.ECS.Doohickey;

public class GuiCharacterCreation : Doohickey, IRenderable {
    public override Uri? Uri { get; } = new Uri( "gui://skill.quest/character/create" );

    public Task Render(){
        throw new NotImplementedException();
    }
}
