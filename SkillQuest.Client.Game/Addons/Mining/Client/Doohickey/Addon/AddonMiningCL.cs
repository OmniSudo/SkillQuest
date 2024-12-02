using SkillQuest.API;
using SQLitePCL;

namespace SkillQuest.Client.Game.Addons.Mining.Client.Doohickey.Addon;

public class AddonMiningCL : Shared.Engine.Addon{
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/mining");

    public AddonMiningCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        
    }

    void OnUnmounted(IAddon addon, IApplication? application){
    }
}
