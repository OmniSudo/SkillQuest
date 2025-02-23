using Godot;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SkillQuest;

public partial class SteamAPI {
    public static uint AppID = 3377440;
    
    public static void LoadLibrary() {
        switch (OS.GetName()) {
            case "Windows":
                if (Engine.GetArchitectureName() == "x86_64") {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/Windows-x64/steam_api64.dll"));
                } else {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/Windows-x64/steam_api.dll"));
                }
                break;
            case "Linux":
                if (Engine.GetArchitectureName() == "x86_64") {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/OSX-Linux-x64/libsteam_api.dll"));
                }
                break;
            case "macOS":
                NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/OSX-Linux-x64/steam_api.bundle/Contents/MacOS/libsteam_api.dylib"));
                break;
        }
    }
}