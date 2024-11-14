using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Game.ECS;

public class Doohickey : Thing, IDoohickey {
    public Doohickey(Uri? uri = null) : base(uri){
    }
}