namespace SkillQuest.Shared.Game;

public class State {
    public static State SH { get; set; } = new State();
    
    public Application Application { get; } = new Application();
}