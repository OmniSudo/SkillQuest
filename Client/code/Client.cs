using Godot;
using System;
using System.Linq;
using System.Net;

namespace SkillQuest;

public partial class Client : Node {
    public static Client CL;
    
    public override void _Ready() {
        CL = this;
        
        if (OS.GetCmdlineArgs().Contains( "--dedicated" )) {
            QueueFree();
            return;
        }

        GD.Print( "Initializing Client" );
        
        try {
            Shared.Multiplayer.Connect( IPEndPoint.Parse( "127.0.0.1:3698" ) ); 
        } catch (Exception e) {
            GD.PrintErr( e );
        }
        
        GD.Print( "Initialized Client Successfully!" );
    }
}