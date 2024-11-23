using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Thing;

public interface IItem : IThing{
    
}

public class Item < TItem > : Engine.ECS.Thing, IItem where TItem : Item< TItem >{
    
}
