using Godot;
using SkillQuest.Network;
using SkillQuest.UI.Login.Select;
using System;
using System.Linq;
using System.Net;

namespace SkillQuest;

public partial class Client : Node {
    public static Client CL;

    public static bool IsClient => !OS.GetCmdlineArgs().Contains( "--dedicated" );

    public static event Connection.Client.DoConnect Connected;

    public static Control UI;
    
    public override void _Ready() {
        if (!IsClient) {
            QueueFree();
            return;
        }

        CL = this;

        GD.Print( "Initializing Client" );

        try {
            UI = GetNode<Control>( "UI" );
            
            Shared.SH.Multiplayer.Connect( IPEndPoint.Parse( "127.0.0.1:3698" ) ).ContinueWith(
                task => {
                    Connected?.Invoke( task.Result );
                }
            );
        } catch (Exception e) {
            GD.PrintErr( e );
        }

        GD.Print( "Initialized Client Successfully!" );
    }
}