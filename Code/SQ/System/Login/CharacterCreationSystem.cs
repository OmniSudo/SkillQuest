using Sandbox.SQ.UI.Character;
using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace Sandbox.SQ.System.Login;

public static class CharacterCreationSystem {
	public static class Client {
		public static void OpenUI ( ) {
			SkillQuest.SH.CreateOnMainThread( SkillQuest.CL.UI.GameObject, true, "character creator" ).ContinueWith(
					task => {
						var create = task.Result.AddComponent< CharacterCreate >();

						void submitted ( CharacterInfo character ) {
							Log.Info( "Submitting " + character  );
							_sv_submit( character );
						}

						create.Submit += submitted;
					}
			);
		}

		public static void CloseUI ( ) {
			var component = SkillQuest.CL.UI.GetComponentInChildren< CharacterCreate >();
			if ( component is null ) { return; }

			SkillQuest.SH.DestroyOnMainThread( component.GameObject );
		}
	}

	public static class Server {
		public static void OpenUI ( Connection connection ) {
			using ( Rpc.FilterInclude( connection ) ) { _cl_open(); }
		}

		public static void CloseUI ( Connection connection ) {
			using ( Rpc.FilterInclude( connection ) ) { _cl_close(); }
		}

		public static event DoCharacterSubmit CharacterSubmitted {
			add => _sv_submitted += value;
			remove => _sv_submitted -= value;
		}
	}
	public delegate void DoCharacterSubmit ( Connection connection, CharacterInfo character );

	private static event DoCharacterSubmit _sv_submitted;

	[ Rpc.Host ]
	private static void _sv_submit ( CharacterInfo character ) {
		if ( CharacterInfo.Server.Exists( Rpc.Caller.SteamId, character.Name ) ) {
			// TODO: Error; Already exists
		} else {
			CharacterInfo.Server.Save( Rpc.Caller.SteamId, character );
			_sv_submitted?.Invoke( Rpc.Caller, character );
			_cl_close();
			CharacterCreationSystem.Server.CloseUI( Rpc.Caller );
			CharacterSelectSystem.Server.OpenUI( Rpc.Caller );
		}
	}

	[ Rpc.Broadcast ]
	private static void _cl_open ( ) {
		Client.OpenUI();
	}

	[ Rpc.Broadcast ]
	private static void _cl_close ( ) {
		Client.CloseUI();
	}
}

