namespace Sandbox.SQ;

public class ServerController : Component, Component.INetworkListener {
	protected override void OnStart ( ) {
		Log.Info( "Created Server" );
	}

	public void OnConnected ( Connection channel ) {
		Log.Info( $"Client {channel.DisplayName} connected" );
	}
}
