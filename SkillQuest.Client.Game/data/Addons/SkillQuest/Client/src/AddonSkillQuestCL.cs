using System.Net;
using SkillQuest.API;
using SkillQuest.Shared.Game;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client;

using static Shared.Game.State;

public class AddonSkillQuestCL : AddonSkillQuestSH{
    public override Uri Uri => new( "cl://addon.skill.quest/skillquest" ) ;

    public new string Description => "Base Game Client";

    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        var task = SH.Net.Connect(IPEndPoint.Parse("127.0.0.1:3698"));
        task.Wait();
        var connection = task.Result;
        Channel.Send( connection, new TestPacket() { Message = Uri.ToString() });
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }
}
