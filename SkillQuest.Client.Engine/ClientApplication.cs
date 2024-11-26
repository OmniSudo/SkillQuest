using System.Net;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using SkillQuest.API;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Shared.Engine;

namespace SkillQuest.Client.Engine;

using static State;

public class ClientApplication : Application{
    public ClientApplication(){
        this.Start += OnStart;
        this.Stop += OnStop;
    }

    bool OnStart(){
        CL.Graphics = new Graphics.Vulkan.VkInstance(
            name: "SkillQuest",
            size: new Vector2D<int>(1280, 720),
            fullscreen: false
        );

        Update += CL.Graphics.Update;
        Render += CL.Graphics.Render;
        CL.Graphics.Quit += () => Running = false;

        return true;
    }

    TimeSpan theta = TimeSpan.Zero;

    protected override void OnUpdate( DateTime now, TimeSpan delta ){
        theta += delta;

        while ( theta > TickFrequency ) {
            base.OnUpdate( now, TickFrequency );
            theta -= TickFrequency;
        }

        OnRender(now, delta);
    }

    bool OnStop(){
        CL.Graphics.Dispose();
        return true;
    }
}
