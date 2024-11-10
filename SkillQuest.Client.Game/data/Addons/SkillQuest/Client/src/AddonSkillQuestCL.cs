using System.Net;
using SkillQuest.Shared.Game;
using SkillQuest.Shared.Game.Addons.Shared.SkillQuest;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client;

using static Shared.Game.State;

public class AddonSkillQuestCL : AddonSkillQuestSH{
    public override string Name => "cl://addon.skill.quest/skillquest";

    public override string Description => "Base Game Client";

    public override string Author => "omnisudo et. all";

    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(Addon addon, IApplication? application){
        var task = SH.Net.Connect(IPEndPoint.Parse("127.0.0.1:3698"));
        task.Wait();
        var client = task.Result;
        client.Send(new TestPacket());
    }

    void OnUnmounted(Addon addon, IApplication? application){ }
}
