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
		public static List< CharacterInfo > GetCharacters ( ) {
			var guid = Guid.NewGuid();
			var tcs  = _tasks [ guid ] = new();

			_sv_getCharactersFromFilesystem( guid );

			return tcs.Task.Result;
		}

		public static CharacterInfo SelectCharacter ( List< CharacterInfo > characters ) {
			Log.Info( "Selecting character" );
			var selector = SkillQuest.CL.UI.AddComponent< CharacterSelect >();

			selector.Characters = characters;

			return characters.FirstOrDefault();
		}
	}

	public static class Server {
		public static async Task< List< CharacterInfo > > GetCharacters ( Connection connection ) {
			var characterListFile = $"server/characters/{connection.SteamId.ToString()}.json";
			var characters = FileSystem.OrganizationData.ReadJsonOrDefault< CharacterInfo [ ] >( characterListFile, [ ] );

			Log.Info(
					$"Loaded {characters.Length} characters from {FileSystem.OrganizationData.GetFullPath( characterListFile )}"
			);

			if ( !FileSystem.OrganizationData.FileExists( characterListFile ) || characters.Length == 0 ) {
				FileSystem.OrganizationData.CreateDirectory( Path.GetDirectoryName( characterListFile ) );

				var tcs = new TaskCompletionSource< CharacterInfo >();

				void ServerOnDisconnected ( Connection c ) {
					Log.Info( $"Aborting character creation wait" );
					tcs.SetResult( null );

					SkillQuest.Server.Disconnected -= ServerOnDisconnected;
				}

				SkillQuest.Server.Disconnected += ServerOnDisconnected;

				var request = CharacterCreationSystem.Server.RequestCreate( connection ).ContinueWith(
						task => {
							Log.Info( $"Received new character creation: {task.Result?.Name}" );
							SkillQuest.Server.Disconnected -= ServerOnDisconnected;
							tcs.SetResult( task.Result );
						}
				);

				var res = await tcs.Task;
				characters = res is null ? [ ] : [ res ];

				FileSystem.OrganizationData.WriteJson( characterListFile, characters );
			}

			return characters.ToList();
		}
	}

	[ Rpc.Host ]
	private static void _sv_getCharactersFromFilesystem ( Guid call ) {
		var caller = Rpc.Caller;
		Server.GetCharacters( caller ).ContinueWith(
				task => {
					Log.Info( "Forwarding characters to client" );
					using ( Rpc.FilterInclude( caller ) ) {
						_cl_doCharacterInfoListCallback( call, task.Result );
					}
				}
		);
	}

	[ Rpc.Broadcast ]
	private static void _cl_doCharacterInfoListCallback ( Guid call, List< CharacterInfo > characters ) {
		Log.Info( "Received characters from server" );

		if ( _tasks.Remove( call, out var tcs ) ) {
			Log.Info( "Finishing up character request task" );
			tcs.SetResult( characters );
		}
	}

	private static Dictionary< Guid, TaskCompletionSource< List< CharacterInfo > > > _tasks = new();
}
