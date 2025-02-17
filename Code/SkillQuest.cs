using Sandbox.Diagnostics;
using Sandbox.SQ;
using Sandbox.SQ.System;
using Sandbox.SQ.System.Login;
using System;
using System.Threading.Tasks;

namespace Sandbox;

public partial class SkillQuest : Component {
	public static Shared SH { get; set; }
	public static Client CL { get; set; }
	public static Server SV { get; set; }

	protected override void OnStart ( ) {
		Log.Info( "Starting Shared" );

		SH = new GameObject( this.GameObject, true, "Shared" ) {
				NetworkMode = NetworkMode.Never
		}.AddComponent< Shared >();
	}

	public class Shared : Component {
		protected override void OnStart ( ) {
			// TODO
			Log.Info( "Started Shared" );

			if ( Networking.IsHost ) {

				Log.Info( "Creating Server" );
				SkillQuest.SV = new GameObject( this.GameObject.Parent, true, "Server" ) {
						NetworkMode = NetworkMode.Never
				}.AddComponent< Server >();

			} else if ( Networking.IsClient ) {

				Log.Info( "Creating Client" );
				SkillQuest.CL = new GameObject( this.GameObject.Parent, true, "Client" ) {
						NetworkMode = NetworkMode.Never
				}.AddComponent< Client >();

			}
		}
	}

	public class Client : Component {
		public ScreenPanel UI { get; set; }

		protected override void OnStart ( ) {
			UI = new GameObject( this.GameObject, true, "UI" ).AddComponent< ScreenPanel >();

			// TODO: Select character to load
			Log.Info( "Requesting Characters" );

			Task.RunInThreadAsync(
							async () => {
								var chars = await CharacterSelectSystem.Client.GetCharacters();
								Log.Info( $"Found {chars.Count} characters" );
							}
					);
			Log.Info( "Started Client" );
		}
	}

	public class Server : Component, Component.INetworkListener {
		public delegate void DoConnected ( Connection connection );
		public static event DoConnected Connected;
		
		public delegate void DoDisconnected ( Connection connection );
		public static event DoDisconnected Disconnected;

		public ScreenPanel UI { get; set; }

		protected override void OnStart ( ) {
			UI = new GameObject( this.GameObject, true, "UI" ) {
					NetworkMode = NetworkMode.Never
			}.AddComponent< ScreenPanel >();

			Log.Info( "Started Server" );
		}

		public void OnConnected ( Connection channel ) {
			Log.Info( $"Client {channel.DisplayName} connected" );

			// TODO: CREATE CHARACTER GAMEOBJECT
			
			try {
				Connected?.Invoke( channel );
			} catch ( Exception e ) {
				Log.Info( $"{channel.DisplayName}\n{e}\n{e.StackTrace}"  );
			}
		}

		public void OnDisconnected ( Connection channel ) {
			Log.Info( $"Client {channel.DisplayName} disconnected" );

			try {
				Disconnected?.Invoke( channel );
			} catch ( Exception e ) {
				Log.Info( $"{channel.DisplayName}\n{e}\n{e.StackTrace}"  );
			}
			// TODO: DELETE CHARACTER GAMEOBJECT
		}
	}
}
