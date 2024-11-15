using SkillQuest.Shared.Game.ECS;
using SkillQuest.Shared.Game.Network;

namespace SkillQuest.Shared.Game;

public class State {
    public static State SH { get; set; } = new State();
    
    public Application Application { get; set; } = new Application();
    
    public Stuff Stuff { get; } = new Stuff();
    
    public Networker Net { get; } = new Networker();
}