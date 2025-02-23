using System;
using System.IO;
using System.Threading.Tasks;

namespace Sandbox.SQ.System.Login;

public class CharacterInfo {
	public SteamId SteamId  { set; get; }
	
	public string  Name     { get; set; }

	public static class Server {
		public static void Save ( SteamId id, CharacterInfo character ) {
			var path = $"server/characters/{id}/{HashCode.Combine( character.Name )}.json";
			FileSystem.OrganizationData.CreateDirectory( Path.GetDirectoryName( path ) );
			FileSystem.OrganizationData.WriteJson( path, character );
		}

		public static CharacterInfo [ ] Load ( SteamId steamid ) {
			var characters = new List<CharacterInfo>();

			foreach ( var name in FileSystem.OrganizationData.FindFile($"server/characters/{steamid}/", "*.json" ) ) {
				Log.Info( name );
				var read = FileSystem.OrganizationData.ReadJsonOrDefault< CharacterInfo >( $"server/characters/{steamid}/{name}", null );
				if ( read is not null ) characters.Add( read );
			}
			
			return characters.ToArray();
		}

		public static CharacterInfo Load ( SteamId steamid, string name ) {
			var info = FileSystem.OrganizationData.ReadJsonOrDefault<CharacterInfo>( $"server/characters/{steamid}/{HashCode.Combine( name )}.json" );
			if ( info == null ) {
				return new CharacterInfo() {
						Name = name,
				};
			}

			return info;
		}

		public static bool Exists ( SteamId steamid, string name ) {
			return FileSystem.OrganizationData.FileExists( $"server/characters/{steamid}/{HashCode.Combine( name )}.json" );
		}
	}
}
