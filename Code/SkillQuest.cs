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

			}

			if ( !Application.IsHeadless ) {
				Log.Info( "Creating Client" );
				SkillQuest.CL = new GameObject( this.GameObject.Parent, true, "Client" ) {
						NetworkMode = NetworkMode.Never
				}.AddComponent< Client >();
			}
		}

		public Task< GameObject > CreateOnMainThread ( GameObject parent, bool enable = true, string name = null ) {
			var tcs = new TaskCompletionSource< GameObject >();

			var request = new CreationRequest( tcs, parent, enable, name );
			creationRequests.Add( request );
			
			return tcs.Task;
		}

		public void DestroyOnMainThread ( GameObject obj ) {
			destroyRequests.Add( obj );
		}

		private struct CreationRequest
				( TaskCompletionSource< GameObject > tcs, GameObject parent, bool enable, string name ) {
			public TaskCompletionSource< GameObject > tcs    = tcs;
			public GameObject                         parent = parent;
			public bool                               enable = enable;
			public string                             name   = name;
		}

		protected override void OnUpdate ( ) {
			if ( creationRequests.Count > 0 ) {
				var requests = creationRequests.ToArray ();
				foreach ( var request in requests ) {
					creationRequests.Remove( request );
					request.tcs.SetResult( new GameObject( request.parent, request.enable, request.name ) );
				}
			}
			
			if ( destroyRequests.Count > 0 ) {
				var requests = destroyRequests.ToArray ();
				foreach ( var request in requests ) {
					request.Destroy();
				}
			}
		}

		private List< CreationRequest > creationRequests = new();

		private List< GameObject > destroyRequests = new();
	}

	public class Client : Component {
		public ScreenPanel UI { get; set; }

		protected override void OnStart ( ) {
			UI = new GameObject( this.GameObject, true, "UI" ) {
					NetworkMode = NetworkMode.Never
			}.AddComponent< ScreenPanel >();

			Log.Info( "Started Client" );
		}
	}

	public class Server : Component, Component.INetworkListener {
		public delegate void DoConnected ( Connection connection );

		public static event DoConnected Connected;

		public delegate void DoDisconnected ( Connection connection );

		public static event DoDisconnected Disconnected;

		public GameObject Clients { get; set; }

		protected override void OnStart ( ) {
			Clients = new GameObject( this.GameObject, true, "Clients" ) {
					NetworkMode = NetworkMode.Never
			};

			Log.Info( "Started Server" );

			OnConnected( Connection.Host );
		}

		public void OnConnected ( Connection channel ) {
			Log.Info( $"Client {channel.DisplayName} connected" );

			// TODO: CREATE CHARACTER GAMEOBJECT
			
			try {
				Connected?.Invoke( channel );
			} catch ( Exception e ) {
				Log.Info( $"{channel.DisplayName}\n{e}\n{e.StackTrace}" );
			}
		}

		public void OnDisconnected ( Connection channel ) {
			Log.Info( $"Client {channel.DisplayName} disconnected" );

			try {
				Disconnected?.Invoke( channel );
			} catch ( Exception e ) {
				Log.Info( $"{channel.DisplayName}\n{e}\n{e.StackTrace}" );
			}
			// TODO: DELETE CHARACTER GAMEOBJECT
		}
	}
}
