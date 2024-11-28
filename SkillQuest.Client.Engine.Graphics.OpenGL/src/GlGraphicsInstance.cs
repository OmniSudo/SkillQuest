using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Drawing;
using System.Xml;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Client.Engine.Graphics.OpenGL;

public class GlGraphicsInstance : IGraphicsInstance {
    public GlGraphicsInstance(IApplication application, string name, Vector2D<int> size, bool fullscreen = false){
        Application = application;

        Silk.NET.Windowing.Window.PrioritizeGlfw();
        Window = Silk.NET.Windowing.Window.Create(WindowOptions.Default with {
            IsEventDriven = false,
            Size = size,
            Title = name,
            UpdatesPerSecond = 0,
            FramesPerSecond = 0,
            // TODO: Fullscreen
        });
        
        Window.Initialize();

        Gui = new ImGuiController(
            Gl = Window.CreateOpenGL(),
            Window,
            Input = Window.CreateInput()
        );
        
        InitializeEvents();
    }

    public IInputContext Input { get; private set; }

    public ImGuiController Gui { get; private set; }

    public IWindow Window { get; private set; }

    void InitializeEvents(){
        Application.Update += Update;
        Application.Render += Render;
        Quit += ( application ) => application.Running = false;
        Window.Closing += OnClose;
        Application.Stuff.ThingAdded += OnStuffThingAdded;
        Application.Stuff.ThingRemoved += OnStuffThingRemoved;
    }

    GlTextureFactory _textureFactory = new();

    public void Draw(RenderPacket packet){
        packet.Texture.Bind();
        packet.Shader.Bind();
        packet.Shader.Attribute( "g_cameraPosition", Vector2D<float>.Zero );
        packet.Shader.Attribute( "g_cameraDirection", Vector3D<float>.UnitY );
        packet.Shader.Attribute("g_model", packet.Model);
        packet.Shader.Attribute("g_view", packet.View);
        packet.Shader.Attribute("g_projection", packet.Projection);
        
        packet.Surface.Draw();
        
        // TODO: Unbind?
    }

    public ITexture CreateTexture(string imagepath){
        return _textureFactory.Build( Gl, imagepath );
    }

    GlModuleFactory _moduleFactory = new();
    
    public IModule CreateModule(string vertexPath, string fragmentPath){
        return _moduleFactory.Build( Gl, vertexPath, fragmentPath );
    }

    GlSurfaceFactory _surfaceFactory = new();
    
    public ISurface CreateSurface(string gltfPath ){
        return _surfaceFactory.Build( Gl, gltfPath );
    }
    
    private ConcurrentDictionary< Uri, IDrawable > _renderables = new();

    void OnStuffThingAdded(IThing thing){
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (thing is IDrawable renderable) {
            _renderables.TryAdd(thing.Uri, renderable);
        }
    }

    void OnStuffThingRemoved(IThing thing){
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (thing is IDrawable renderable) {
            if (_renderables.TryGetValue(thing.Uri, out var old) && old == renderable) {
                _renderables.TryRemove(thing.Uri, out _);
            }
        }
    }

    void OnClose(){
        Quit.Invoke(Application);
    }

    public IApplication Application { get; private set; }

    public Glfw Glfw { get; private set; }

    public GL Gl { get; private set; }

    public void Update(DateTime now, TimeSpan delta){
        Window.DoUpdate();
        Window.DoEvents();
    }

    public void Render(DateTime now, TimeSpan delta){
        unsafe {
            Gui.Update( ( float ) delta.TotalSeconds );
            
            Gl.ClearColor( Color.Black );
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            
            var clone = _renderables.ToImmutableDictionary();
            foreach (var pair in clone) {
                pair.Value.Draw();
            }
            
            Gui.Render();
            
            Window.SwapBuffers();
        }
    }

    public event IGraphicsInstance.DoQuit? Quit;
    
    public void Dispose(){
        Gl.Dispose();
    }
}
