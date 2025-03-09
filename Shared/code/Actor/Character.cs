using Godot;

namespace SkillQuest.Actor;

public partial class Character : Node {
    public class Info {
        public string Name { get; set; }
    }
    
    public Info About { get; init; }
}