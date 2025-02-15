namespace Sandbox.SQ;

public class ServerController : Component, Component.INetworkListener {
	protected override void OnStart ( ) {
		SkillQuest.Client = new GameObject( this.GameObject.Parent, true, "Client" );
		Log.Info( "Created Server" );
	}

	public void OnConnected ( Connection channel ) {
		Log.Info( $"Client {channel.DisplayName} connected" );

		var client = new GameObject( SkillQuest.Client, true, channel.DisplayName );
		client.AddComponent< ClientConnection >( ).Connection = channel;
	}

	public void OnDisconnected ( Connection channel ) {
		Log.Info( $"Client {channel.DisplayName} disconnected" );

		var client = SkillQuest.Client.Children.SingleOrDefault( go => go.Name == channel.DisplayName );
		client.Destroy();
	}
}
