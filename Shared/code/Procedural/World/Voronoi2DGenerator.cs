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
        var st = new SurfaceTool();

        st.Begin( Mesh.PrimitiveType.Lines );
        var voronoi = new VoronoiPolygons( 0, 15 );
        var pos = new Vector2( region.Position.X, region.Position.Y );
        voronoi.Add( new Rect( pos, pos + Vector2.One * 16 ) );
        
        foreach (var polygon in voronoi.At( new Rect( pos, pos + Vector2.One * 16 ) )) {
            foreach (var edge in polygon.Edges ) {
                st.AddVertex( new Vector3( edge.PointA.X, 0, edge.PointA.Y) );
                st.AddVertex( new Vector3( edge.PointB.X, 0, edge.PointB.Y) );
            }
        }
        
        var mesh = st.Commit();
        var meshinstance = new MeshInstance3D();
        meshinstance.Name = region.Position.ToString();
        meshinstance.SetMesh(mesh);
        Shared.SH.CallDeferred( () => {
            region.AddChild( meshinstance );
        } );
        Console.WriteLine( meshinstance.Name );
    }
}