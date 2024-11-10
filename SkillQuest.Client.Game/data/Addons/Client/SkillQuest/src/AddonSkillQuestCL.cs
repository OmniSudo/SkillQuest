using SkillQuest.Shared.Game;

namespace SkillQuest.Client.Game.Addons.Client.SkillQuest;

public class AddonSkillQuestCL : Addon {
    public override string Name { get; protected set; } = "cl://addon.skill.quest/skillquest";

    public override string Description { get; protected set; } = "Base Game";

    public override string Author { get; protected set; } = "o mnisudo et. all";
    
    public AddonSkillQuestCL(){
        
    }
}