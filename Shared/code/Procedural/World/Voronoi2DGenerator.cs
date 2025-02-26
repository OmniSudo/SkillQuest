using Godot;
using SkillQuest.Procedural.Node;
using SkillQuest.World;

namespace SkillQuest.Procedural.World;

public partial class Voronoi2DGenerator : Godot.Node {
    public override void _Ready() {
        GetParent<EntryPointNode>().Generate += Generate;
    }

    public void Generate(Region region) {
        var st = new SurfaceTool();
        
        st.Begin( Mesh.PrimitiveType.Triangles );
        
        st.AddIndex(0);
        st.AddIndex(1);
        st.AddIndex(2);

        st.AddIndex(0);
        st.AddIndex(2);
        st.AddIndex(3);
        
        st.AddVertex( new Vector3(0, 0, 0) );
        st.AddVertex( new Vector3(1, 0, 0) );
        st.AddVertex( new Vector3(1, 0, 1) );
        st.AddVertex( new Vector3(0, 0, 1) );
        
        var mesh = st.Commit();
        var meshinstance = new MeshInstance3D();
        meshinstance.SetMesh(mesh);
        region.AddChild(meshinstance);
    }
}