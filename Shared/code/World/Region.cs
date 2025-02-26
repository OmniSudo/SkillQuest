using Godot;

namespace SkillQuest.World;

public partial class Region : Node3D {
    public Universe Universe { get; set; }

    public Region(Universe universe, Vector3 position) {
        Universe = universe;
        Position = position;
    }

    public override void _Ready() {
        
    }
}