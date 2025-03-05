using Godot;
using SkillQuest.API.Geometry;
using SkillQuest.API.Geometry.Random;
using SkillQuest.Procedural.Node;
using SkillQuest.World;
using System;
using System.Linq;

namespace SkillQuest.Procedural.World;

public partial class Voronoi2DGenerator : Godot.Node {
    public override void _Ready() {
        GetParent<EntryPointNode>().Generate += Generate;
    }

    public async void Generate(Region region) {
        
    }
}