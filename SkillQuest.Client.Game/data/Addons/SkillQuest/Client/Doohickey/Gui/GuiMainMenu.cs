using ImGuiNET;
using SkillQuest.API.ECS;

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
            ImGui.Text("Hello, world!");
            ImGui.End();
        }
    }
}
