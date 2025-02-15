using System.Data;

namespace Sandbox.SQ;

public sealed class ClientConnection : Component {
	[Property] public Connection Connection { get; set; }
}
