using Godot;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SkillQuest;

public partial class SteamAPI : Node {
    public static uint AppID = 3377440;
    
    public override void _Ready() {
        switch (OS.GetName()) {
            case "Windows":
                if (Engine.GetArchitectureName() == "x86_64") {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/win64/steam_api64.dll"));
                } else {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/steam_api.dll"));
                }
                break;
            case "Linux":
                if (Engine.GetArchitectureName() == "x86_64") {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/linux64/steam_api.so"));
                } else {
                    NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/linux32/steam_api.so"));
                }
                break;
            case "macOS":
                NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "Shared/lib/steamworks/osx/libsteam_api.dylib"));
                break;
        }
    }
}