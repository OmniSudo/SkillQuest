using Godot;
using System;
using System.Linq;

namespace SkillQuest;

public partial class Server : Node {
    public static Server SV;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        SV = this;

        if (!OS.GetCmdlineArgs().Contains( "--server" )) {
            return;
        }

        GD.Print( "Initializing Server" );

        try {
            Shared.Multiplayer.Host( 3698 );
        } catch (Exception e) {
            GD.PrintErr( e );
        }

        GD.Print( "Initialized Server Successfully!" );
    }

    ~Server() {
        foreach (var (ip, server) in Shared.Multiplayer.Servers) {
            server.Shutdown();
        }
    }

    private bool initialized = false;

    public override void _ExitTree() {
        if (!initialized) return;

        try {
            
        } catch (Exception e) {
            GD.PrintErr( e );
        }
    }
}