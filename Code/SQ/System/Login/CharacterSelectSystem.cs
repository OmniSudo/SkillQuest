using Sandbox.SQ.Actor;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.SQ.System.Login;

public static class CharacterSelectSystem {
	public static class Client {
		public static async Task< List< CharacterInfo > > GetCharacters ( ) {
			var guid = Guid.NewGuid();
			var tcs  = _tasks [ guid ] = new();
			
			_sv_getCharactersFromFilesystem( guid );
			
			return await tcs.Task;
		}
	}

	public static class Server {
		public static async Task< List< CharacterInfo > > GetCharacters ( Connection connection ) {
			var characterListFile = $"server/characters/{connection.SteamId.ToString()}.json";
			var characters =
					FileSystem.OrganizationData.ReadJsonOrDefault< List< CharacterInfo > >( characterListFile, [ ] );

			if ( !FileSystem.Data.FileExists( characterListFile ) || characters.Count == 0 ) {
				FileSystem.OrganizationData.CreateDirectory( Path.GetDirectoryName( characterListFile ) );

				var tcs = new TaskCompletionSource< CharacterInfo  >();

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
				
				FileSystem.OrganizationData.WriteJson< List< CharacterInfo > >(
						characterListFile, characters = ( res is null ? [ ] : [ res ] )
				);
			}
			
			Log.Info( $"Loaded character list for user {connection.Name}"  );
			return characters;
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
