using Microsoft.VisualBasic;
using Sandbox.SQ.Actor;
using Sandbox.SQ.UI.Character;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.SQ.System.Login;

public static class CharacterSelectSystem {
	public static class Client {
		public static void OpenUI ( CharacterInfo [ ] characters ) {
			var comp = SkillQuest.CL.UI.GetComponentInChildren< CharacterSelect >();
			if ( comp is null ) {
				SkillQuest.SH.CreateOnMainThread( SkillQuest.CL.UI.GameObject, true, "character creator" ).ContinueWith(
						task => {
							var create = task.Result.AddComponent< CharacterSelect >();

							void selected ( CharacterInfo character ) {
								_sv_submit( character );
							}

							create.Characters = characters;
							create.Selected += selected;
						}
				);
			} else {
				comp.Enabled = true;
			}
		}

		public static void CloseUI ( ) {
			var component = SkillQuest.CL.UI.GetComponentInChildren< CharacterSelect >();
			if ( component is null ) { return; }

			SkillQuest.SH.DestroyOnMainThread( component.GameObject );
		}
	}

	public static class Server {
		public static void OpenUI ( Connection connection ) {
			var characters = CharacterInfo.Server.Load( connection.SteamId );
			using ( Rpc.FilterInclude( connection ) ) { _cl_open( characters ); }
		}

		public static void CloseUI ( Connection connection ) {
			using ( Rpc.FilterInclude( connection ) ) { _cl_close(); }
		}

		public static event DoCharacterSelect CharacterSelected {
			add => _sv_selected += value;
			remove => _sv_selected -= value;
		}
	}

	public delegate void DoCharacterSelect ( Connection connection, CharacterInfo character );

	private static event DoCharacterSelect _sv_selected;

	[ Rpc.Host ]
	private static void _sv_submit ( CharacterInfo character ) {
		if ( CharacterInfo.Server.Exists( Rpc.Caller.SteamId, character.Name ) ) {
			// TODO: Error; Already exists
		} else {
			_sv_selected?.Invoke( Rpc.Caller, character );
		}
	}

	[ Rpc.Broadcast ]
	private static void _cl_open ( CharacterInfo [ ] characters ) {
		Client.OpenUI( characters );
	}

	[ Rpc.Broadcast ]
	private static void _cl_close ( ) {
		Client.CloseUI();
	}
}
