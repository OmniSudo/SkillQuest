using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using SkillQuest.API;
using SkillQuest.Shared.Game;

namespace SkillQuest.Client.Game;

public class ClientApplication : Application {
    public override void Loop(){
        window = Window.Create(WindowOptions.Default);

        window.Load += () => {
            imgui = new ImGuiController(
                gl = window.CreateOpenGL(),
                window,
                input = window.CreateInput()
            );
        };

        window.Update += d => {
            Update?.Invoke();
        };

        window.Render += d => {
            imgui.Update((float)d);

            gl.ClearColor(0, 0, 0, 255);
            gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
            
            Render?.Invoke();
            
            imgui.Render();
        };

        window.Closing += () => {
            imgui?.Dispose();
            input?.Dispose();
            gl?.Dispose();
        };

        window.Run();
    }

    public delegate void DoRender();
    
    public event DoRender Render;
    
    public event IApplication.DoStart? Start;

    public event IApplication.DoUpdate? Update;

    public event IApplication.DoStop? Stop;
    
    IWindow window;
    ImGuiController imgui;
    GL gl;
    IInputContext input;
}
