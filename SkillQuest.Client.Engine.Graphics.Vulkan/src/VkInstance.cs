using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.SDL;
using SkillQuest.Client.Engine.Graphics.API;
using Monitor = Silk.NET.GLFW.Monitor;

namespace SkillQuest.Client.Engine.Graphics.Vulkan;

public class VkInstance : IInstance {
    private unsafe WindowHandle* _window = null;

    public VkInstance( string name, Vector2D<int> size, bool fullscreen = false ){
        unsafe {
            Name = name;
            Size = size;
        
            Glfw = Glfw.GetApi();
            
            Glfw.SetErrorCallback(ErrorCallback);
            
            if ( !Glfw.Init() ) throw new ArgumentNullException( nameof( Glfw ) );
        
            Glfw.WindowHint( WindowHintClientApi.ClientApi, ClientApi.NoApi );
            
            _window = Glfw.CreateWindow( size.X, size.Y, name, default, default );
        }
    }

    void ErrorCallback(ErrorCode error, string description){
        Console.WriteLine( $"{error}: {description}" );
    }
    
    public void Update( DateTime now, TimeSpan delta ){
        unsafe {
            if (Glfw.WindowShouldClose(_window)) {
                Quit?.Invoke();
            }
            
            Glfw.PollEvents();
        }
    }

    public void Render(DateTime now, TimeSpan delta){
        
    }

    public event IInstance.DoQuit? Quit;

    public void CreateDevice(){
        
    }

    public string Name { get; set; }

    public Vector2D<int> Position { get; set; }

    public Vector2D<int> Size { get; set; }

    public Glfw Glfw { get; }

    public void Dispose(){
        unsafe {
            Glfw.DestroyWindow( _window );
            _window = null;
            Glfw.Terminate();
            Glfw.Dispose();
        }
    }
}
