using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.SDL;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using SkillQuest.Client.Engine.Graphics.API;
using Monitor = Silk.NET.GLFW.Monitor;
using Version = System.Version;

namespace SkillQuest.Client.Engine.Graphics.Vulkan;

public class VkInstance : IInstance{

    public VkInstance(string name, Vector2D<int> size, bool fullscreen = false, bool validate = false){
        Name = name;
        Size = size;
        EnableValidationLayers = validate;
        Fullscreen = fullscreen;

        InitializeWindow();
    }

    public bool Fullscreen { get; set; }

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

        var appInfo = new ApplicationInfo() {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version12,
            ApplicationVersion = new Version32(0, 0, 0),
            EngineVersion = new Version32(0, 0, 0),
            PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(Name),
            PEngineName = (byte*)Marshal.StringToHGlobalAnsi(Name)
        };

        byte** requiredExtensions =
            (byte**)SilkMarshal.StringArrayToPtr(GetRequiredExtensions(out uint extensionCount));

        byte** validationLayers = (byte**)SilkMarshal.StringArrayToPtr(
            GetValidationLayers(
                new[] {
                    "VK_LAYER_KHRONOS_validation"
                },
                out uint layerCount
            )
        );

        DebugUtilsMessengerCreateInfoEXT? debugCreateInfo = null;
        PopulateDebugInfo(ref debugCreateInfo);

        var instanceCreateInfo = new InstanceCreateInfo() {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledExtensionCount = extensionCount,
            PpEnabledExtensionNames = requiredExtensions,
            EnabledLayerCount = layerCount,
            PpEnabledLayerNames = validationLayers,
            PNext = debugCreateInfo is null ? null : &debugCreateInfo
        };

        var result = Vk.CreateInstance(&instanceCreateInfo, null, out vkInstance);

        if (result != Result.Success) {
            SilkMarshal.Free((IntPtr)instanceCreateInfo.PpEnabledExtensionNames);
            SilkMarshal.Free((IntPtr)instanceCreateInfo.PpEnabledLayerNames);
            Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
            Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);

            throw new Exception($"Failed to create Vulkan instance: {result}");
        }

        Console.WriteLine("Vulkan instance created successfully");

        SilkMarshal.Free((IntPtr)instanceCreateInfo.PpEnabledExtensionNames);
        SilkMarshal.Free((IntPtr)instanceCreateInfo.PpEnabledLayerNames);
        Marshal.FreeHGlobal((IntPtr)appInfo.PApplicationName);
        Marshal.FreeHGlobal((IntPtr)appInfo.PEngineName);

        if (debugCreateInfo is not null) {
            Vk.TryGetInstanceExtension(vkInstance, out ExtDebugUtils debugUtilsMessengerEXT);

            debugUtilsMessengerEXT!.CreateDebugUtilsMessenger(vkInstance, debugCreateInfo!.Value, null,
                out var debugMessenger);
            
            Console.WriteLine( "Debug layer initialized successfully" );
        }
    }

    unsafe void PopulateDebugInfo(ref DebugUtilsMessengerCreateInfoEXT? createInfo){
#if DEBUG
        if (!EnableValidationLayers) {
            return;
        }
        
        unsafe {
            createInfo = new() {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)VulkanDebugCallback
            };
        }
#endif
    }

    private unsafe uint VulkanDebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData){
        Console.WriteLine($"validation layer:" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));

        return Vk.False;
    }

    string[] GetValidationLayers(string[] requested, out uint count){
#if DEBUG
        if (!EnableValidationLayers) {
            count = 0;
            return [];
        }

        unsafe {
            uint available = 0;
            Vk.EnumerateInstanceLayerProperties(&available, null);
            LayerProperties[] availableLayers = new LayerProperties[(int)available];

            fixed (LayerProperties* availableLayersPtr = availableLayers) {
                Vk.EnumerateInstanceLayerProperties(&available, availableLayersPtr);
            }

            var names = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();

            if (!requested.All(names.Contains)) throw new Exception("Failed to find all validation layers");

            count = (uint)requested.Length;
            return requested;
        }
#endif
        count = 0;
        return [];
    }

    public bool EnableValidationLayers { get; set; } = false;

    unsafe string[] GetRequiredExtensions(out uint count){
        var extensions = Glfw.GetRequiredInstanceExtensions(out count);
        var result = new List<string>((int)count + 1);

        for (int i = 0; i < count; i++) {
            result.Add(Marshal.PtrToStringAnsi((IntPtr)extensions[i]));
        }
        result.Add(ExtDebugUtils.ExtensionName);

        SilkMarshal.Free((IntPtr)extensions);
        return result.ToArray();
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

    public Vector2D<int> Size {
        get {
            return _size;
        }
        set {
            unsafe {
                _size = value;

                if (_window is not null && !Fullscreen) {
                    Glfw.SetWindowSize(_window, _size.X, _size.Y);
                }
            }
        }
    }

    private Vector2D<int> _size = Vector2D<int>.Zero;

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

    public void CheckSuccess(Result result, Action<Result> onFail = null){
        if (result != Result.Success) {
            onFail?.Invoke(result);
            throw new Exception($"Vulkan error: {result}");
        }
    }
}
