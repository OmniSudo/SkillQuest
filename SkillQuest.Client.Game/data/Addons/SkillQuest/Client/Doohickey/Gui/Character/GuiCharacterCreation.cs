using System.Numerics;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;

using Doohickey = Shared.Engine.ECS.Doohickey;

public class GuiCharacterCreation : Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("gui://skill.quest/character/create");

    private CharacterCreator _creator;

    private IClientConnection _connection;

    public GuiCharacterCreation(IClientConnection connection){
        _creator = new CharacterCreator(connection);
        _connection = connection;
    }

    string name;

    Task<bool> Created;
    
    public void Render(){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings
            )
        ) {
            ImGui.InputTextWithHint( "Name", "name", ref name, 128 );
            
            ImGui.Separator();
            if (
                ImGui.Button("Create")
            ) {
                Created = Task.Run(async () => {
                    if (!await _creator.IsNameAvailable(name)) {
                        return false;
                    }
                    
                    var character = await _creator.CreateCharacter(name);
                    
                    if (character == null) {
                        Console.WriteLine("Unable To Create Character");
                        return false;
                    }
                    
                    Console.WriteLine("\nCharacter Created: " + character.CharacterId + " (" + character.Name + ")");

                    Stuff?.Remove( _creator );
                    _creator.Reset();
                    Stuff?.Remove(this);
        
                    Stuff?.Add( new GuiCharacterSelection( _connection ) ).Render();
                    return true;
                });
            }
            if (
                ImGui.Button("Cancel")
            ) {
                Task.Run(async () => {
                        Stuff?.Remove( _creator );
                        _creator.Reset();
                        Stuff?.Remove(this);
        
                        Stuff?.Add( new GuiCharacterSelection( _connection ) );
                });
            }
            ImGui.End();
        }

        if (Created.IsCompleted && !Created.Result) {
            ImGui.TextColored( new Vector4( 1.0f, 0, 0, 0 ), $"{name} has already been taken" );
        }
        ImGui.End();
    }
}
