using System.Net;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiInGame : Shared.Engine.ECS.Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("ui://skill.quest/ingame");
    
    private IPlayerCharacter _localhost;
    
    public GuiInGame( IPlayerCharacter localhost ){
        _localhost = localhost;
    }

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

            if (ImGui.Button($"WIN @ {_localhost.Name}")) {
                Console.WriteLine( $"YOU WON {_localhost.Name}");
                _localhost.Connection.Disconnect();
                Stuff.Add(new GuiMainMenu());
                Stuff?.Remove(this);
            }
            ImGui.End();
        }
    }
}
