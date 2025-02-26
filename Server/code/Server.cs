using Godot;
using SkillQuest.Network;
using SkillQuest.UI.Login.Select;
using System;
using System.Linq;

namespace SkillQuest;

public partial class Server : Node {
    public static Server SV;

    public static bool IsHost => OS.GetCmdlineArgs().Contains( "--server" );

    public static bool IsDedicated => OS.GetCmdlineArgs().Contains( "--dedicated" );

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        if (!IsHost) {
            QueueFree();
            return;
        }

        SV = this;

        GD.Print( "Initializing Server" );

        try {
            var server = Shared.Multiplayer.Bind( 3698 );
            server.Connected += (s, c) => {
                CharacterSelect.Server.GetSelection( c ).ContinueWith( task => {
                    for (var x = -16; x < 16; x++) {
                        for (var y = -16; y < 16; y++) {
                            Universe.World.Generate( c, new Vector3( x, y, 0 ) );
                        }
                    }
                } );
            };
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