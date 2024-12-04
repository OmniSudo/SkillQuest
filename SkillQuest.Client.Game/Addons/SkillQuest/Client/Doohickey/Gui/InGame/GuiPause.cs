using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiPause : Shared.Engine.ECS.Doohickey, IDrawable, IHasControls {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame/pause");

    public GuiPause(){
        ConnectInput();
    }
    
    public void Draw(DateTime now, TimeSpan delta){
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
            if (ImGui.Button($"Admin Editor")) {
                foreach (var gui in Stuff.Things.Where(g => g.Key.Scheme == "ui")) {
                    if (gui.Value == this) continue;
                    
                    Stuff.Remove(gui.Value);
                }
                Stuff.Add(new GuiEditor());
                Stuff.Remove(this);
            }

            ImGui.End();
        }
    }

    public void ConnectInput(){
        Engine.State.CL.Keyboard.KeyDown += KeyboardOnKeyDown;
    }

    void KeyboardOnKeyDown(IKeyboard arg1, Key key, int arg3){
        if (key == Key.Escape) {
            DisconnectInput();
            Stuff.Remove(this);
        }
    }

    public void DisconnectInput(){
        Engine.State.CL.Keyboard.KeyDown -= KeyboardOnKeyDown;
    }
}
