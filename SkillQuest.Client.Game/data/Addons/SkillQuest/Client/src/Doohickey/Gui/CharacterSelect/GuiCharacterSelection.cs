using SkillQuest.API.ECS;
using SkillQuest.API.Network;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.CharacterSelect;

using Doohickey = Shared.Game.ECS.Doohickey;

public class GuiCharacterSelection : Doohickey, IRenderable {
    public override Uri? Uri { get; } = new Uri( "gui://skill.quest/character/select" );

    readonly IClientConnection _connection;
    
    Character.CharacterSelect _characterSelect;

    public GuiCharacterSelection(IClientConnection connection){
        _connection = connection;
        _characterSelect = new Character.CharacterSelect(_connection);
    }
    
    public async Task Render(){
        var characters = await _characterSelect.Characters();

        if (characters.Length > 0) {
            var selected = await _characterSelect.Select( characters.First() );
            Console.WriteLine( "Selected {0}", selected?.Name );
        }
    }
}
