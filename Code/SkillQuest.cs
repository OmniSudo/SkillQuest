using Sandbox.Diagnostics;

namespace Sandbox;

public class SkillQuest : Component {
	public GameObject SharedPrefab { get; set; } = GameObject.GetPrefab( "shared/shared.prefab" );
	public GameObject ServerPrefab { get; set; } = GameObject.GetPrefab( "server/server.prefab" );
	public GameObject ClientPrefab { get; set; } = GameObject.GetPrefab( "client/client.prefab" );

	public static GameObject Shared { get; set; }
	public static GameObject Client { get; set; }
	public static GameObject Server { get; set; }

	protected override void OnStart ( ) {
		Log.Info( "Starting Shared" );

		Shared      = SharedPrefab.Clone( Vector3.Zero );
		Shared.Name = "Shared";
		Shared.SetParent( this.GameObject );
	}
}
