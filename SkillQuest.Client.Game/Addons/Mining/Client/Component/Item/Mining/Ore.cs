using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Thing;

namespace SkillQuest.Client.Game.Addons.Mining.Client.Thing.Item.Mining;

public class Ore : Component< Ore >{
    public virtual Material Material { get; set; } = null;
    
    
}
