using Godot;
using System;
using System.Collections.Generic;

namespace SkillQuest.Procedural.World.Room;

[Tool]
public partial class StoneRoom : Node3D {
    private Vector2I _size = new Vector2I(1, 1);

    [Export]
    public Vector2I Size {
        get => _size;
        set {
            _size = value;
            
            CreateFloor();
            CreateWalls();
        }
    } 

    public override void _Ready() {
        // CreateFloor();
        //CreateWalls();
    }

    private void CreateWalls() {
        var node = GetNodeOrNull( "Wall" );
        if ( node is not null ) RemoveChild( node );
        node?.Dispose();
        node = new Node3D() { Name = "Wall" };
        AddChild( node );
        
        var wall = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/wall.gltf" );
        var half = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/wall_half.gltf" );
        var corner = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/wall_corner.gltf" );

        for (int x = 1; x < Size.X - 1; x += 2) {
            if (x + 1 < this.Size.X - 1) {
                var l = wall.Instantiate<Node3D>();
                l.Position = new Vector3( x * 2 + 2, 0, 0 );
                l.Name = $"Wall_{x}_0";
                node.AddChild( l );
            } else {
                var s = half.Instantiate<Node3D>();
                s.Position = new Vector3( x * 2, 0, 0 );
                s.Name = $"Wall_{x}_0";
                node.AddChild( s );
            }
        }

        for (int x = 1; x < Size.X - 1; x += 2) {
            if (x + 1 < this.Size.X - 1) {
                var l = wall.Instantiate<Node3D>();
                l.Position = new Vector3( x * 2 + 2, 0, Size.Y * 2 );
                l.Name = $"Wall_{x}_0";
                node.AddChild( l );
            } else {
                var s = half.Instantiate<Node3D>();
                s.Position = new Vector3( x * 2, 0, Size.Y * 2 );
                s.Name = $"Wall_{x}_0";
                node.AddChild( s );
            }
        }
        
        for (int y = 1; y < Size.Y - 1; y += 2) {
            if (y + 1 < this.Size.Y - 1) {
                var l = wall.Instantiate<Node3D>();
                l.Position = new Vector3( 0,0,y * 2 + 2 );   
                l.RotationDegrees = new Vector3( 0, -90, 0 );
                l.Name = $"Wall_0_{y}";
                node.AddChild( l );
            } else {
                var s = half.Instantiate<Node3D>();
                s.Position = new Vector3(0,0, y * 2 );   
                s.RotationDegrees = new Vector3( 0, -90, 0 );
                s.Name = $"Wall_0_{y}";
                node.AddChild( s );
            }
        }

        for (int y = 1; y < Size.Y - 1; y += 2) {
            if (y + 1 < this.Size.Y - 1) {
                var l = wall.Instantiate<Node3D>();
                l.Position = new Vector3( Size.X * 2,0,y * 2 + 2 );   
                l.RotationDegrees = new Vector3( 0, -90, 0 );
                l.Name = $"Wall_{Size.X}_{y}";
                node.AddChild( l );
            } else {
                var s = half.Instantiate<Node3D>();
                s.Position = new Vector3(Size.X * 2,0, y * 2 );   
                s.RotationDegrees = new Vector3( 0, -90, 0 );
                s.Name = $"Wall_{Size.X}_{y}";
                node.AddChild( s );
            }
        }

        var cornerSW = corner.Instantiate<Node3D>();
        cornerSW.Name = "cornerSW";
        cornerSW.RotationDegrees = new Vector3( 0, 90, 0 );
        node.AddChild( cornerSW );
        
        var cornerNW = corner.Instantiate<Node3D>();
        cornerNW.Name = "cornerNW";
        cornerNW.Position = new Vector3( Size.X * 2, 0, 0 );
        cornerNW.RotationDegrees = new Vector3( 0, 0, 0 );
        node.AddChild( cornerNW );

        var cornerSE = corner.Instantiate<Node3D>();
        cornerSE.Name = "cornerSE";
        cornerSE.Position = new Vector3( 0, 0, (Size.Y) * 2 );
        cornerSE.RotationDegrees = new Vector3( 0, 180, 0 );
        node.AddChild( cornerSE );
        
        var cornerNE = corner.Instantiate<Node3D>();
        cornerNE.Name = "cornerNE";
        cornerNE.Position = new Vector3( Size.X * 2, 0, (Size.Y) * 2 );
        cornerNE.RotationDegrees = new Vector3( 0, -90, 0 );
        node.AddChild( cornerNE );
    }

    public void CreateFloor() {
        var node = GetNodeOrNull( "Floor" );
        if ( node is not null ) RemoveChild( node );
        node?.Dispose();
        node = new Node3D() { Name = "Floor" };
        AddChild( node );
        
        var large = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/floor_tile_large.gltf" );
        var small = GD.Load<PackedScene>( "res://Shared/assets/props/dungeon/floor_tile_small.gltf" );

        for (int x = 0; x < this.Size.X; x += 2) {
            for (int y = 0; y < this.Size.Y; y += 2) {
                if (x + 1 < this.Size.X && y + 1 < this.Size.Y) {
                    var l = large.Instantiate<Node3D>();
                    l.Position = new Vector3( x * 2 + 2f, 0, y * 2 + 2f );
                    l.Name = $"Floor_{x}_{y}";
                    node.AddChild( l );
                } else {
                    if (x + 1 == Size.X) {
                        for (var i = 0; i <( ( y + 1 == Size.Y ) ? 1 : 2); i++) {
                            var s = small.Instantiate<Node3D>();
                            s.Position = new Vector3( x * 2 + 1, 0f, (y + i)* 2 + 1 );
                            s.Name = $"Floor_{x + i}_{y}";
                            node.AddChild( s );
                        }
                    }
                    if (y + 1 == Size.Y) {
                        for (var i = 0; i <( ( x + 1 == Size.X ) ? 0 : 2); i++) {
                            var s = small.Instantiate<Node3D>();
                            s.Position = new Vector3( ( x + i ) * 2 + 1, 0f, y* 2 + 1 );
                            s.Name = $"Floor_{x + i}_{y}";
                            node.AddChild( s );
                        }
                    }
                }
            }
        }
    }
}