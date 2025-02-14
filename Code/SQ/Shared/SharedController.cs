using Sandbox.Diagnostics;
using System.IO;

namespace Sandbox.SQ;

public sealed class SharedController : Component {
	protected override void OnStart ( ) {
		Log.Info( "Created Shared" );
		
		if ( Networking.IsHost ) {
			Log.Info( "Starting Server" );

			var prefab = GameObject.GetComponentInParent< SkillQuest >( ).ServerPrefab;
			var server = prefab.Clone( Vector3.Zero );

			server.Name = "Server";
			server.SetParent( this.GameObject.Parent );
		} else if ( Networking.IsClient ) {
			Log.Info( "Starting Client" );

			var prefab = GameObject.GetComponentInParent< SkillQuest >( ).ClientPrefab;
			var client = prefab.Clone( Vector3.Zero );

			client.Name = "Client";
			client.SetParent( this.GameObject.Parent );
		}
	}
}
