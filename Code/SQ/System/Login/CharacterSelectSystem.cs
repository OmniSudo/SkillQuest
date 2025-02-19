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
		public static Task< CharacterInfo > GetSelection ( CharacterInfo [ ] characters ) {
			return CharacterSelect.SelectCharacter( characters );
		}
	}

	public static class Server {
		public static Task< CharacterInfo [ ] > GetCharacters ( Connection connection ) {
			Log.Info( "Loading characters from JSON file"  );
			var path = $"server/users/{connection.SteamId}.json";

			CharacterInfo[] characters = FileSystem.OrganizationData.ReadJsonOrDefault< CharacterInfo[] >( path, [] );
			var tcs  = new TaskCompletionSource< CharacterInfo [ ] >();

			if ( !FileSystem.OrganizationData.FileExists( path ) || characters.Length == 0 ) {
				FileSystem.OrganizationData.CreateDirectory( Path.GetDirectoryName( path ) );

				var task = CharacterCreationSystem.Server.RequestCreate( connection );
				task.ContinueWith(
						( t ) => {
							Log.Info( $"Created new character {t.Result.Name}" );
							FileSystem.OrganizationData.WriteJson( path, characters );
							tcs.SetResult( [ t.Result ] );
						}
				);

				void disconnected ( Connection connection ) {
						tcs.SetResult( [ ] );
						SkillQuest.Server.Disconnected -= disconnected;
				}
				
				SkillQuest.Server.Disconnected += disconnected;
			}

			

			if ( characters.Length > 0 ) {
				tcs.SetResult( characters );
			}
			
			return tcs.Task;
		}

		public static Task< CharacterInfo > RequestSelect ( Connection connection ) {
			var guid = Guid.NewGuid();
			var tcs = ( TaskCompletionSource< CharacterInfo > ) (
					_tasks [ guid ] = new TaskCompletionSource< CharacterInfo >()
			);
			
			void disconnected ( Connection connection ) {
				tcs.SetResult( null );
				SkillQuest.Server.Disconnected -= disconnected;
			}
			
			SkillQuest.Server.Disconnected += disconnected;
			
			GetCharacters( connection ).ContinueWith(
					task => {
						Log.Info( $"Got characters from connection {connection.DisplayName}" );
						using ( Rpc.FilterInclude( connection ) ) {
							_cl_getSelection( guid, task.Result );
						}
					}
			);

			return tcs.Task;
		}
	}

	[ Rpc.Broadcast ]
	private static void _cl_getSelection ( Guid guid, CharacterInfo [ ] characters ) {
		Client.GetSelection( characters ).ContinueWith(
				task => {
					Log.Info( "Got selection" );
					using ( Rpc.FilterInclude( Connection.Host ) ) {
						_sv_gotSelection( guid, task.Result );
					}
				}
		);
	}

	[ Rpc.Host ]
	private static void _sv_gotSelection ( Guid guid, CharacterInfo info ) {
		if ( _tasks.Remove( guid, out var result ) ) {
			var task = ( TaskCompletionSource< CharacterInfo > ) result;
			if ( !task.Task.IsCompleted ) {
				task.SetResult( info );
			}
		}
	}

	private static Dictionary< Guid, object > _tasks = new();
}
