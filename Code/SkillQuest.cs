using Sandbox.Diagnostics;

namespace Sandbox;

public class SkillQuest : Component {
	public GameObject SharedPrefab { get; set; }
	
	public GameObject ServerPrefab { get; set; }
	
	public GameObject ClientPrefab { get; set; }

	protected override void OnStart ( ) {
		Log.Info( "Starting Shared" );
		
		SharedPrefab = GameObject.GetPrefab( "shared/shared.prefab" );
		ServerPrefab = GameObject.GetPrefab( "server/server.prefab" );
		ClientPrefab = GameObject.GetPrefab( "client/client.prefab" );
		
		var shared = SharedPrefab.Clone( Vector3.Zero );

		shared.Name = "Shared";
		shared.SetParent( this.GameObject );
	}
}
