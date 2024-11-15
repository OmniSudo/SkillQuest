using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Thing.Item;

public interface IItem : IThing{
    
}

public class Item < TItem > : ECS.Thing, IItem where TItem : Item< TItem >{
    
}
