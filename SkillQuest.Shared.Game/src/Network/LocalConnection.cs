using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lidgren.Network;
using SkillQuest.API.Network;
using Timer = System.Timers.Timer;

namespace SkillQuest.Shared.Game.Network;

using static State;

public class LocalConnection : ILocalConnection {
    public LocalConnection(
        ServerConnection server,
        IPEndPoint endpoint
    ){
        Server = server;
        EndPoint = endpoint;
        _timeout = new Timer( TimeSpan.FromSeconds( 10 ) );
        _timeout.Elapsed += (sender, args) => {
            Server.Disconnect(this);
            _timeout.Enabled = false;
        };
    }

    public IServerConnection Server { get; }
    
    public INetworker Networker => Server?.Networker ?? SH.Net;

    public IPEndPoint EndPoint { get; }

    public NetConnection Connection { get; private set; }

    public byte[] Key { get; set; }

    NetAESEncryption AES { get; set; }

    Timer _timeout;

    public void Send(IPacket packet, bool udp = false ){
        var serialized = JsonSerializer.Serialize(packet);

        NetOutgoingMessage message = Connection.Peer.CreateMessage();
        message.Write(packet.GetType().FullName);

        message.Write(serialized);
        message.Encrypt(AES);
        Connection.SendMessage( message, udp ? NetDeliveryMethod.Unreliable : NetDeliveryMethod.ReliableOrdered, 0 );
    }

    public void Connect(NetConnection netConnection){
        Connection = netConnection;
        AES = new NetAESEncryption( Connection.Peer );
        AES.SetKey( Key, 0, Key.Length );
    }

    public void InterruptTimeout(){
        _timeout.Enabled = false;
    }

    public void Disconnect(){
        Connection.Disconnect( "CLIENT DISCONNECT" );
        Disconnected?.Invoke(this);
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    public void Connect(IPEndPoint endpoint){
        throw new NotImplementedException();
    }

    void Receive(IClientConnection connection, IPacket packet){
        Console.WriteLine( connection.EndPoint.ToString() );
        Console.WriteLine( JsonSerializer.Serialize( packet ) );
    }
}
