using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;

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

        var selection = await DoSelect( characters );
        
        if (selection == characters.Length) {
            _characterSelect.Reset();
            Stuff!.Remove(_characterSelect);
            Stuff!.Remove(this);

            _ = new GuiCharacterCreation( _connection ).Render();
        } else {
            var selected = await _characterSelect.Select( characters[ selection ] );
            Console.WriteLine( "Selected {0}", selected?.Name );
        }
    }

    public async Task<int> DoSelect( IPlayerCharacter[] characters ){
        ConsoleKey key;
        int selection = 0;

        do {
            Console.Clear();
            var width = Console.BufferWidth;
            Console.WriteLine( "Select A Character" );
            
            
            for ( var i = 0; i < characters.Length; i++ ) {
                if (i == selection) {
                    Console.Write( "\t> " );
                } else {
                    Console.Write( "\t  " );
                }
                Console.Write( characters[i].Name );
                if (i == selection) {
                    Console.WriteLine( " <" );
                } else {
                    Console.WriteLine();
                }
            }
        
            Console.WriteLine( ( selection == characters.Length ? "> " : "  " ) + "Create A New Character" + ( selection == characters.Length ? " <" : "" ));
            
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.UpArrow) {
                selection--;
                if (selection < 0 ) selection = characters.Length;
            } else if (key == ConsoleKey.DownArrow) {
                selection++;
                if ( selection > characters.Length ) selection = 0;
            }
        } while ( key != ConsoleKey.Enter );
        
        return selection;
    }
}
