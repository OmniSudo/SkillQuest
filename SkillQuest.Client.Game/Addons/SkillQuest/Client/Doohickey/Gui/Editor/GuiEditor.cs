using System.Numerics;
using ImGuiNET;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiEditor : Shared.Engine.ECS.Doohickey, IDrawable {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/editor");

    public void Draw(DateTime now, TimeSpan delta){
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetNextWindowPos(new Vector2(0, 0));

        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove
            )
        ) {
            if (ImGui.Button($"Test")) {
                Console.WriteLine("Test");
            }

            ImGui.End();
        }
    }
}