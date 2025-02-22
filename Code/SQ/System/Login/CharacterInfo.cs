using System;
using System.Threading.Tasks;

namespace Sandbox.SQ.System.Login;

public class CharacterInfo {
	public SteamId SteamId  { set; get; }
	
	public string  Name     { get; set; }

	public static class Server {
		public static void Save ( CharacterInfo character ) {
			FileSystem.OrganizationData.WriteJson(
					$"server/characters/{HashCode.Combine( character.Name )}.json", character
			);
		}

		public static CharacterInfo Load ( string name ) {
			var info = FileSystem.OrganizationData.ReadJsonOrDefault<CharacterInfo>( $"server/characters/{HashCode.Combine( name )}.json" );
			if ( info == null ) {
				return new CharacterInfo() {
						Name = name,
				};
			}

			return info;
		}

		public static bool Exists ( string name ) {
			return FileSystem.OrganizationData.FileExists( $"server/characters/{HashCode.Combine( name )}.json" );
		}
	}

	public static class Client {
		public static Task< bool > Exists ( string name ) {
			var guid = Guid.NewGuid();
			var tcs  = new TaskCompletionSource< bool >();
			_tasks [ guid ] = tcs;

			_sv_requestDoesCharacterExist( guid, name );
			
			return tcs.Task;
		}
	}
	
	[ Rpc.Host ]
	private static void _sv_requestDoesCharacterExist ( Guid call, string name ) {
		using ( Rpc.FilterInclude( Rpc.Caller ) ) {
			_cl_doesCharacterExist( call, Server.Exists( name ) );
		}
	}

	[ Rpc.Broadcast ]
	private static void _cl_doesCharacterExist ( Guid call, bool exists ) {
		if ( !_tasks.Remove( call, out var tcs ) ) return;

		tcs.SetResult( exists );
	}

	private static Dictionary< Guid, TaskCompletionSource< bool > > _tasks = new();
}
