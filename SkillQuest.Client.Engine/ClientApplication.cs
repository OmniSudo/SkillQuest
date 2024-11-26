using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Client.Engine;

using static State;

public class ClientApplication : Application{
    public override void Loop(){
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1280, 720);
        window = Window.Create(options);

        window.Load += () => {
            
        };

        window.Update += d => {
            OnUpdate();
        };

        window.Render += d => {
            OnRender();
        };

        window.Closing += () => {
            input?.Dispose();
        };

        window.Run();
    }

    IWindow window;
    IInputContext input;
}
