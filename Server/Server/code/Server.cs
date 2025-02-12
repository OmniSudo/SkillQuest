using Godot;
using Steamworks;
using System;
using System.Net;

namespace SkillQuest;

public partial class Server : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		try {
			SteamServer.Init( SteamAPI.AppID, new SteamServerInit() {
				DedicatedServer = true,
				GameDescription = "SkillQuest",
				GamePort = 3698,
				IpAddress = IPAddress.Any,
				ModDir = "SkillQuest",
				VersionString = "0.0.0.0",
				SteamPort = 9000,
				Secure = true,
				QueryPort = 36987
			} );
			SteamServer.OnSteamServersConnected += SteamServerOnOnSteamServersConnected;

			SteamServer.ServerName = "SkillQuest";
			
			SteamServer.LogOnAnonymous();
		} catch (Exception e) {
			GD.PrintErr(e);
		}
	}

	private void SteamServerOnOnSteamServersConnected() {
		GD.Print("Steam server connected");
	}

	public override void _ExitTree() {
		try {
			SteamServer.LogOff();
		} catch (Exception e) {
			GD.PrintErr(e);
		}
	}
}