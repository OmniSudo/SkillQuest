using Godot;
using System;
using System.Collections.Generic;

namespace SkillQuest.Procedural.World.Room;

public partial class StoneRoom : Node3D {
    [Export] public Vector2I Size;

    public override void _Ready() {
        CreateFloor();
    }

    public void CreateFloor() {
        var floor = new Node3D() { Name = "Floor" };
        AddChild( floor );
        var large = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/floor_tile_large.gltf" );
        var small = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/floor_tile_small.gltf" );

        for (int x = 0; x <= this.Size.X; x += 2) {
            for (int y = 0; y <= this.Size.Y; y += 2) {
                if (x + 1 < this.Size.X && y + 1 < this.Size.Y) {
                    var l = large.Instantiate<Node3D>();
                    l.Position = new Vector3( x * 2 + 2f, x, y * 2 + 2f );
                    l.Name = $"Floor_{x}_{y}";
                    floor.AddChild( l );
                } else {
                    for (var i = 0; i < 2; i++) {
                        if (x + i + 1 <= Size.X) {
                            var s = small.Instantiate<Node3D>();
                            s.Position = new Vector3( (x + i) * 2 + 1, 0f, y * 2 + 1 );
                            s.Name = $"Floor_{x + i}_{y}";
                            floor.AddChild( s );
                        } else if (y + i + 1 <= Size.Y) {
                            var s = small.Instantiate<Node3D>();
                            s.Position = new Vector3( x * 2 + 1, 0f, (y + i) * 2 + 1 );
                            s.Name = $"Floor_{x}_{y + i}";
                            floor.AddChild( s );
                        }
                    }
                }
            }
        }
    }
}