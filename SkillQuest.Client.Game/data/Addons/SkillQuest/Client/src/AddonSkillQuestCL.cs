using SkillQuest.Shared.Game;
using SkillQuest.Shared.Game.Addons.Shared.SkillQuest;

namespace SkillQuest.Client.Game.Addons.Client.SkillQuest;

public class AddonSkillQuestCL : AddonSkillQuestSH {
    public override string Name => "cl://addon.skill.quest/skillquest";

    public override string Description => "Base Game Client";

    public override string Author => "omnisudo et. all";
    
    public AddonSkillQuestCL(){
        Mounted += OnMounted;   
        Unmounted += OnUnmounted;
    }

    void OnMounted(Addon addon, IApplication? application){
        
    }

    void OnUnmounted(Addon addon, IApplication? application){
        
    }
}