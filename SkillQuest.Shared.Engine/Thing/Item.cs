using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Thing;

public interface IItem : IThing{
    
}

public class Item < TItem > : Engine.ECS.Thing, IItem where TItem : Item< TItem >{
    
}
