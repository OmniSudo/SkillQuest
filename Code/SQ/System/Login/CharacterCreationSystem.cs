using Sandbox.SQ.UI.Character;
using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace Sandbox.SQ.System.Login;

public static class CharacterCreationSystem {
	public static class Client {
		public static Task< CharacterInfo > Create ( ) {
			Log.Info( "Starting create character UI" );
			
			return CharacterCreate.CreateCharacter();
		}
	}

	public static class Server {
		public static Task< CharacterInfo > RequestCreate ( Connection connection ) {
			Log.Info( $"Creating new character on {connection.DisplayName}"  );
			var guid = Guid.NewGuid();
			var tcs  = _tasks [ guid ] = new();

			using ( Rpc.FilterInclude( connection ) ) {
				__createCharacter( guid );
			}

			return tcs.Task;
		}

		public static bool CharacterExists ( string name ) {
			return false;
		}
	}


	[ Rpc.Broadcast ]
	private static void __createCharacter ( Guid call ) {
		var ret = Client.Create().ContinueWith(
				t => {
					var ret = t.Result;
					
					ret.SteamId = Connection.Local.SteamId;

					Log.Info( HashCode.Combine( ret.Name ) ) ;

					__didCreateCharacter( call, ret );
				}
		);
	}

	[ Rpc.Host ]
	private static void __didCreateCharacter ( Guid call, CharacterInfo character ) {
		if ( !_tasks.Remove( call, out var tcs ) ) return;

		tcs.SetResult( character );
	}

	private static Dictionary< Guid, TaskCompletionSource< CharacterInfo > > _tasks = new();
}
