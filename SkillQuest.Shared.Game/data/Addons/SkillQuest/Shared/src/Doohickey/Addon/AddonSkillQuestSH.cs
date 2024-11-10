using System.ComponentModel;
using SkillQuest.Shared.Game.ECS;
using IComponent = SkillQuest.Shared.Game.ECS.IComponent;

namespace SkillQuest.Shared.Game.Addons.Shared.SkillQuest;

public class AddonSkillQuestSH : Addon{
    public AddonSkillQuestSH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(Addon addon, IApplication? application){
        
    }

    void OnUnmounted(Addon addon, IApplication? application){
        
    }
}
