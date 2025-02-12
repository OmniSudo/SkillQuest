using Godot;
using Steamworks;
using System;

namespace SkillQuest;

public partial class Client : Node {
    public override void _Ready() {
        try {
            SteamClient.Init( SteamAPI.AppID );
        } catch (Exception e) {
            GD.PrintErr( e.Message );
            GetTree().Quit();
        }
    }
}