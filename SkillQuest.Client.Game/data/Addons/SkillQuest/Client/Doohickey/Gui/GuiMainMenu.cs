using System.Net;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.Network;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui;

public class GuiMainMenu : Shared.Engine.ECS.Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("ui://skill.quest/mainmenu");
    
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
            if (ImGui.Button("Connect")) {
                SH.Net.Connect(IPEndPoint.Parse("127.0.0.1:3698"));
            }
            ImGui.End();
        }
    }
}
