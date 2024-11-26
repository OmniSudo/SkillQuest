using Silk.NET.GLFW;
using Silk.NET.Maths;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Client.Engine.Graphics.Vulkan;

public class VkInstance : IInstance {
    public VkInstance( string name, Vector2D<int> size, bool fullscreen = false, Vector2D<int>? position = null ){
        Name = name;
        Size = size;
        
        Glfw = Glfw.GetApi();
        if ( !Glfw.Init() ) throw new ArgumentNullException( nameof( Glfw ) );
        
        Position = position ?? Vector2D<int>.Zero;
    }

    public void CreateDevice(){
        
    }

    public string Name { get; set; }

    public Vector2D<int> Position { get; set; }

    public Vector2D<int> Size { get; set; }

    public Glfw Glfw { get; }

    public void Dispose(){
        Glfw.Dispose();
    }
}
