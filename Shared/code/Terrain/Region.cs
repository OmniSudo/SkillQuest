using Godot;
using SkillQuest;

namespace SkillQuest.Terrain;

public partial class Region : Node3D {
    public World World { get; set; }

    public Region(World world, Vector3 position) {
        World = world;
        Position = position;
    }

    public override void _Ready() {
        
    }
}