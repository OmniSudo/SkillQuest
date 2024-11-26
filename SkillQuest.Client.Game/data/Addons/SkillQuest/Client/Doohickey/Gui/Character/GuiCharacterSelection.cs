using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;

using Doohickey = Shared.Engine.ECS.Doohickey;

public class GuiCharacterSelection : Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("gui://skill.quest/character/select");

    readonly IClientConnection _connection;

    Client.Doohickey.Character.CharacterSelect _characterSelect;
    readonly Task<IPlayerCharacter[]> _characters;

    public GuiCharacterSelection(IClientConnection connection){
        _connection = connection;
        _characterSelect = new Client.Doohickey.Character.CharacterSelect(_connection);
        
        _characters = _characterSelect.Characters();
    }

    public void Render(){
        

        if (_characters.IsCanceled) {
            Console.WriteLine("Unable to download character list...");
            // TODO: Recover from this

            Stuff!.Remove(this);
            Stuff!.Remove(_characterSelect);
            var login = Stuff.Add( new GuiMainMenu() );
            Authenticator.Instance.Logout(_connection);

            return;
        }

        IPlayerCharacter selection = null;
        if (_characters.IsCompleted) {
            selection = DoSelect(_characters.Result);
        }

        if (ImGui.Button("Create")) {
            _characterSelect.Reset();
            Stuff.Add( new GuiCharacterCreation(_connection) );
            
            Stuff!.Remove(_characterSelect);
            Stuff!.Remove(this);

            return;
        }
        
        if (selection != null) {
            Task.Run( async () => {
                var selected = await _characterSelect.Select(selection);
                if (selected is null) return;
                
                selected.Connection = _connection;
                
                Console.WriteLine("Selected {0}", selected?.Name);

                Stuff!.Add(new GuiInGame(selected!));
                
                _characterSelect.Reset();
                Stuff!.Remove(_characterSelect);
                Stuff!.Remove(this);
            });
        }
    }

    public IPlayerCharacter DoSelect(IPlayerCharacter[] characters){
        IPlayerCharacter? ret = null;
        
        if ((characters?.Length ?? 0 )== 0) {
            return ret;
        }
        
        foreach (var character in characters) {
            if (ImGui.Button(character.Name)) {
                ret = character;
            }
        }
        return ret;
    }
}
