using System.Dynamic;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.CharacterSelect;

using Doohickey = Shared.Game.ECS.Doohickey;

public class GuiCharacterCreation : Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("gui://skill.quest/character/create");

    private CharacterCreator _creator;

    private IClientConnection _connection;

    public GuiCharacterCreation(IClientConnection connection){
        _creator = new CharacterCreator(connection);
    }

    public async Task Render(){

        create:
        
        var name = string.Empty;
        while ( name.Length == 0 ) {
            Console.Clear();
            Console.Write("name > ");
            ConsoleKey key;

            do {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && name.Length > 0) {
                    Console.Write("\b \b");
                    name = name[0..^1];
                } else if (!char.IsControl(keyInfo.KeyChar)) {
                    Console.Write(keyInfo.KeyChar);
                    name += keyInfo.KeyChar;
                }
            } while ( key != ConsoleKey.Enter );
        }

        if (!await _creator.IsNameAvailable(name)) {
            Console.WriteLine();
            Console.WriteLine("Invalid Character Name");
            
            await Task.Delay( TimeSpan.FromSeconds(5) );

            goto create;
        }
        
        var character = await _creator.CreateCharacter(name);

        if (character == null) {
            Console.WriteLine();
            Console.WriteLine("Unable To Create Character");
           
            await Task.Delay( TimeSpan.FromSeconds(5) );

            goto create;
        }
        
        Console.WriteLine("\nCharacter Created: " + character.CharacterId + " (" + character.Name + ")");

        Stuff.Remove( _creator );
        _creator.Reset();
        Stuff.Remove(this);
        
        _ = new GuiCharacterSelection( _connection ).Render();
    }
}
