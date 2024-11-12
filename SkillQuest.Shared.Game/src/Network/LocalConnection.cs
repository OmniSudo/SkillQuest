using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lidgren.Network;
using SkillQuest.API.Network;
using Timer = System.Timers.Timer;

namespace SkillQuest.Shared.Game.Network;

using static State;

internal class LocalConnection : ILocalConnection{
    public LocalConnection(
        ServerConnection server,
        IPEndPoint endpoint
    ){
        Server = server;
        EndPoint = endpoint;
        _timeout = new Timer(TimeSpan.FromSeconds(10));

        _timeout.Elapsed += (sender, args) => {
            Server.Disconnect(this);
            _timeout.Enabled = false;
        };
    }

    public IServerConnection Server { get; }

    public INetworker Networker => Server?.Networker ?? SH.Net;

    public IPEndPoint EndPoint { get; }

    public NetConnection Connection { get; private set; }

    public string EMail { get; set; }

    public Guid Id { get; set; }

    public string AuthToken { get; set; }

    public Guid Session { get; set; }

    public byte[] Key { get; set; }

    internal NetEncryption Encryption { get; set; }

    Timer _timeout;

    public void Send(Packet packet, bool udp = false){
        var serialized = JsonSerializer.Serialize(packet, packet.GetType());

        NetOutgoingMessage message = Connection.Peer.CreateMessage();
        var bytes = Encoding.UTF8.GetBytes(packet.GetType().FullName);
        message.WriteVariableInt32(bytes.Length);
        message.Write(bytes);

        bytes = Encoding.UTF8.GetBytes(serialized);
        message.WriteVariableInt32(bytes.Length);
        message.Write(bytes);

        if (message.Encrypt(Encryption)) {
            Connection.SendMessage(
                message,
                udp ? NetDeliveryMethod.Unreliable : NetDeliveryMethod.ReliableOrdered,
                0
            );
            
        }
    }

    public void Connect(NetConnection netConnection){
        Connection = netConnection;
        Encryption = new NetXtea(Connection.Peer, Key);
    }

    public void InterruptTimeout(){
        _timeout.Enabled = false;
    }

    public void Disconnect(){
        Connection.Disconnect("CLIENT DISCONNECT");
        Disconnected?.Invoke(this);
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;

    public void Connect(IPEndPoint endpoint){
        throw new InvalidOperationException();
    }

    public void Receive(Packet packet){
        Networker.Channels.TryGetValue(packet.Channel, out var channel);
        channel?.Receive(this, packet);
    }
}
