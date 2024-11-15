using System.Net;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using SkillQuest.API;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Command;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Login;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;
using SkillQuest.Shared.Game;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client;

using static Shared.Game.State;

public class AddonSkillQuestCL : AddonSkillQuestSH{
    public override Uri Uri => new("cl://addon.skill.quest/skillquest");

    public new string Description => "Base Game Client";

    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        Commands = Stuff.Add(new CommandRegistry());
        
        var connection = SH.Net.Connect(IPEndPoint.Parse("127.0.0.1:3698")).Result;

        if (connection is not null) {
            Stuff.Add(new GuiLoginSignup(connection)).Render().Wait();
        }

        application.Update += () => {
            Commands.Listen().Wait();
        };
    }

    public Authenticator Authenticator { get; private set; }

    public static CommandRegistry Commands { get; private set; }
    
    void OnUnmounted(IAddon addon, IApplication? application){ }
    
}
