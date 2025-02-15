using Sandbox.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace Sandbox.SQ;

public sealed class SharedController : Component {
	protected override void OnStart ( ) {
		Log.Info( "Created Shared" );
		
		if ( Networking.IsHost ) {
			Log.Info( "Starting Server" );

			var prefab = GameObject.GetComponentInParent< SkillQuest >( ).ServerPrefab;
			SkillQuest.Server = prefab.Clone( Vector3.Zero );

			SkillQuest.Server.Name = "Server";
			SkillQuest.Server.SetParent( this.GameObject.Parent );
		} else if ( Networking.IsClient ) {
			Log.Info( "Starting Client" );

			var prefab = GameObject.GetComponentInParent< SkillQuest >( ).ClientPrefab;
			SkillQuest.Client = prefab.Clone( Vector3.Zero );

			SkillQuest.Client.Name = "Client";
			SkillQuest.Client.SetParent( this.GameObject.Parent );
		}
	}
}
