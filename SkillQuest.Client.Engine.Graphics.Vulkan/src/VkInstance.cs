using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.SDL;
using Silk.NET.Vulkan;
using SkillQuest.Client.Engine.Graphics.API;
using Monitor = Silk.NET.GLFW.Monitor;
using Version = System.Version;

namespace SkillQuest.Client.Engine.Graphics.Vulkan;

public class VkInstance : IInstance{

    public VkInstance(string name, Vector2D<int> size, bool fullscreen = false){
        Name = name;
        Size = size;

        InitializeWindow();
    }

    private void InitializeWindow(){
        InitializeGLFW();

        Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

        unsafe {
            _window = Glfw.CreateWindow(Size.X, Size.Y, Name, default, default);
        }

        InitializeVulkan();
    }

    private void InitializeGLFW(){
        Glfw = Glfw.GetApi();

        Glfw.SetErrorCallback(ErrorCallback);

        if (!Glfw.Init()) throw new ArgumentNullException(nameof(Glfw));
    }

    private unsafe void InitializeVulkan(){
        Vk = Vk.GetApi();

        var appInfo = new ApplicationInfo() with {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version12,
            ApplicationVersion = new Version32(0, 0, 0),
            EngineVersion = new Version32(0, 0, 0),
            PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(Name),
            PEngineName = (byte*)Marshal.StringToHGlobalAnsi(Name)
        };

        var instanceCreateInfo = new InstanceCreateInfo() with {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledExtensionCount = 0,
            PpEnabledExtensionNames = null,
            EnabledLayerCount = 0,
            PpEnabledLayerNames = null
        };

        var result = Vk.CreateInstance(&instanceCreateInfo, null, out vkInstance);

        if (result != Result.Success) {
            Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
            Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);

            throw new Exception($"Failed to create Vulkan instance: {result}");
        } else {
            Console.WriteLine("Vulkan instance created successfully");
        }

        Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
        Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);
    }

    void ErrorCallback(ErrorCode error, string description){
        Console.WriteLine($"{error}: {description}");
    }

    public void Update(DateTime now, TimeSpan delta){
        unsafe {
            if (Glfw.WindowShouldClose(_window)) {
                Quit?.Invoke();
            }

            Glfw.PollEvents();
        }
    }

    public void Render(DateTime now, TimeSpan delta){ }

    public event IInstance.DoQuit? Quit;

    public void CreateDevice(){ }

    public string Name { get; set; }

    public Vector2D<int> Position { get; set; }

    public Vector2D<int> Size { get; set; }

    public Glfw Glfw { get; private set; }

    public Vk Vk { get; private set; }

    private unsafe WindowHandle* _window = null;

    public Instance vkInstance;

    public unsafe IntPtr WindowHandle => (IntPtr)_window;

    public void Dispose(){
        unsafe {
            Glfw.DestroyWindow(_window);
            _window = null;
            Glfw.Terminate();
            Glfw.Dispose();
        }
    }
}
