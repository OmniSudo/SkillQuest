using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace Sandbox.SQ.System.Login;

public static class CharacterCreationSystem {
	public static class Client {
		public static async Task< CharacterInfo > Create ( ) {
			// TODO: Wait for UI
			await Task.Delay ( 0 );
			
			return new CharacterInfo() {
					Name = "Exoa Ilter"
			};
		}
	}

	public static class Server {
		public static Task< CharacterInfo > RequestCreate ( Connection connection ) {
			var guid = Guid.NewGuid();
			var tcs  = _tasks [ guid ] = new();

			using ( Rpc.FilterInclude( connection ) ) {
				__createCharacter( guid );
			}

			return tcs.Task;
		}
	}


	[ Rpc.Broadcast ]
	private static void __createCharacter ( Guid call ) {
		var ret = Client.Create().Result;

		ret.SteamId = Rpc.Caller.SteamId;

		__didCreateCharacter( call, ret );
	}

	[ Rpc.Host ]
	private static void __didCreateCharacter ( Guid call, CharacterInfo character ) {
		if ( !_tasks.Remove( call, out var tcs ) ) return;

		tcs.SetResult( character );
	}

	private static Dictionary< Guid, TaskCompletionSource< CharacterInfo > > _tasks = new();
}
