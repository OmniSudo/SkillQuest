using SkillQuest.API;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;

using static State;

public class AddonSkillQuestSH : Game.Addon {
    public override string Name { get;} = "SkillQuest";

    public override string Author { get; } = "omnisudo et. all";

    protected IChannel Channel { get; private set; }
    
    public AddonSkillQuestSH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        Channel = SH.Net.CreateChannel(new Uri("net://skill.quest/test"));
        Channel.Subscribe< TestPacket >((connection, packet) => Console.WriteLine(packet.Message));
    }

    void OnUnmounted(IAddon addon, IApplication? application){
        
    }
}
