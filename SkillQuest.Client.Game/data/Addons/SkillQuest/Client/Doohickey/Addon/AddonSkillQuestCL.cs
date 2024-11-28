using System.Collections.Immutable;
using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Addon;

public class AddonSkillQuestCL : AddonSkillQuestSH {
    public override Uri Uri { get; set; } = new Uri("cl://addon.skill.quest/skillquest");

    public override string Description { get; } = "Base Game";
    
    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        Stuff.Add(new GuiMainMenu());
    }


    void OnUnmounted(IAddon addon, IApplication? application){ }
}
