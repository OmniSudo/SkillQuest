using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Network;

namespace SkillQuest.Shared.Engine;

public class State {
    public static State SH { get; set; } = new State();
    
    public Application Application { get; set; } = new Application();
    
    public Stuff Stuff { get; } = new Stuff();
    
    public Networker Net { get; } = new Networker();
}