using Godot;
using SkillQuest.Packet;
using SkillQuest.Packet.System;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace SkillQuest.Network;

public abstract class Connection {
    public class Server {
        public Multiplayer Multiplayer { get; }

        public IPEndPoint EndPoint { get; }

        public static bool IsSelf(Connection.Client client) {
            return client is Local;
        }

        public ImmutableDictionary<IPEndPoint, Connection.Client> Clients => _clients.ToImmutableDictionary();

        ConcurrentDictionary<IPEndPoint, Connection.Client> _clients = new();

        public Server(Multiplayer multiplayer, short port) {
            Multiplayer = multiplayer;
            EndPoint = new IPEndPoint( IPAddress.Any, port );

            _server = new TcpListener( EndPoint );
            _server.Start();

            GD.Print( $"Listening @ {EndPoint}" );
        }

        ~Server() {
            _server.Stop();
        }
        
        public TcpListener _server { get; private set; }

        public RSA RSA { get; } = new RSACryptoServiceProvider();

        Thread _thread;

        public void Update(double delta) {
            if (_server.Pending()) {
                try {
                    var client = _server.AcceptTcpClient();
                    GD.Print( $"Accepted @ {client.Client.RemoteEndPoint}" );

                    var connection =
                        _clients[
                            client.Client.RemoteEndPoint as IPEndPoint ??
                            throw new ArgumentNullException( nameof(client.Client.RemoteEndPoint) )
                        ] = new Connection.Local( this, client );

                    connection.Disconnected += clientConnection => {
                        Disconnected?.Invoke( this, clientConnection );
                        _clients.TryRemove( clientConnection.EndPoint, out var _ );
                    };
                    
                    (connection as Connection.Local)?.Ready();
                } catch (Exception e) {
                    GD.PrintErr( $"Unable to accept connection {e}" );
                }
            }


        }
        
        protected internal void OnConnected(Connection.Client connection) {
            Connected?.Invoke( this, connection );
            GD.Print( $"Connected @ {connection.EndPoint}" );
        }

        public delegate void DoConnected(Connection.Server server, Connection.Client client);

        public event DoConnected Connected;
        
        protected internal void OnDisconnected(Connection.Client connection) {
            Disconnected?.Invoke( this, connection );
            GD.Print( $"Disconnected @ {connection.EndPoint}" );
        }

        public delegate void DoDisconnected(Connection.Server server, Connection.Client endpoint);

        public event DoDisconnected? Disconnected;

        public void Disconnect(Connection.Client connection) {
            _clients.TryRemove( connection.EndPoint, out var client );

            Disconnected?.Invoke( this, connection );
            client?.Disconnect();
        }

        public void Shutdown() {
            foreach (var client in _clients.Values) {
                client.Disconnect();
            }
            
            _server.Stop();
        }
    }

    public class Client {
        protected internal TcpClient? _client { get; set; } = null;

        protected NetworkStream? _stream;
        
        public string Username {
            get {
                var steamid = new CSteamID(this.SteamId);
                bool needsToRetreiveInformationFromInternet = SteamFriends.RequestUserInformation(steamid, true);
                if (!needsToRetreiveInformationFromInternet)
                {
                    return SteamFriends.GetFriendPersonaName(steamid);
                }

                return "nil";
            }
        }

        public ulong SteamId { get; set; }

        public string AuthToken { get; set; }

        public virtual RSA RSA { get; } = new RSACryptoServiceProvider();

        public Aes AES;

        public bool Running { get; set; }

        public async Task Send(Packet packet, bool encrypt = true) {
            var serialized = JsonSerializer.Serialize( packet, packet.GetType() );

            var typename = packet.GetType().FullName;

            byte[] ciphertext = [];

            if (encrypt) {
                ICryptoTransform encryptor = AES.CreateEncryptor( AES.Key, AES.IV );
                var bytes_typename = Array.Empty<byte>();

                using (var msEncrypt = new MemoryStream()) {
                    using (var csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write )) {
                        byte[] plainBytes = Encoding.UTF8.GetBytes( typename );
                        csEncrypt.Write( plainBytes, 0, plainBytes.Length );
                    }

                    var b64 = Convert.ToBase64String( msEncrypt.ToArray() );
                    bytes_typename = Encoding.ASCII.GetBytes( b64 ).ToArray();
                }

                var bytes_packet = Array.Empty<byte>();

                using (var msEncrypt = new MemoryStream()) {
                    using (var csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write )) {
                        byte[] plainBytes = Encoding.UTF8.GetBytes( serialized );
                        csEncrypt.Write( plainBytes, 0, plainBytes.Length );
                    }

                    var b64 = Convert.ToBase64String( msEncrypt.ToArray() );
                    bytes_packet = Encoding.ASCII.GetBytes( b64 ).ToArray();
                }

                ciphertext = new byte[] { (byte)0xF0 }
                    .Concat( bytes_typename )
                    .Concat( [(byte)0x00] )
                    .Concat( bytes_packet )
                    .Concat( [(byte)0x00] )
                    .ToArray();
                _stream.Write( ciphertext, 0, ciphertext.Length );
            } else {
                ciphertext = new byte[] { (byte)0x0F }
                    .Concat( Encoding.UTF8.GetBytes( typename ) )
                    .Concat( [(byte)0x00] )
                    .Concat( Encoding.UTF8.GetBytes( serialized ) )
                    .Concat( [(byte)0x00] )
                    .ToArray();
                _stream.Write( ciphertext, 0, ciphertext.Length );
            }
        }

        public virtual void InterruptTimeout() {
            throw new NotImplementedException();
        }

        public void Disconnect() {
            var ep = EndPoint;
            GD.Print( $"Disconnecting @ {ep}" );
            OnDisconnect();
            GD.Print( $"Disconnected @ {ep}" );
            _client?.Client?.Disconnect( false );
            _client?.Client?.Shutdown( SocketShutdown.Both );
            _stream = null;
            _client?.Dispose();
            _client = null;
            GD.Print( $"Disposed @ {ep}" );
        }
        
        protected internal virtual void Ready() {
            _stream = _client.GetStream();

            _keepalive = new Timer( TimeSpan.FromSeconds( 20 ) );
            _keepalive.Elapsed += (sender, args) => {
                if (Status == Connection.State.Disconnecting) {
                    Disconnect();
                    _keepalive.Dispose();
                }
            };
            _keepalive.Start();
        }

        protected internal void OnConnected() {
            Connected?.Invoke( this );
        }

        protected internal void OnDisconnect() {
            Disconnected?.Invoke( this );
        }

        public delegate void DoConnect(Connection.Client connection);

        public event DoConnect? Connected;

        public delegate void DoDisconnect(Connection.Client connection);

        public event DoDisconnect? Disconnected;

        private IEnumerable<byte> buffer = Array.Empty<byte>();
        int delimiters = 0;
        Timer _keepalive;

        private bool PendingSplit(byte b) {
            return b != 0x00;
        }

        public bool Receive() {
            bool completed = false;

            while (_stream?.DataAvailable ?? false) {
                var data = new byte[1024];
                var len = _stream.Read( data, 0, data.Length );
                data = data.Take( len ).ToArray();

                do {
                    var take = data.SkipWhile( (b) => b == 0 ).TakeWhile( PendingSplit );
                    if (take.Count() == 0) break;

                    var leftover = len - take.Count();

                    buffer = buffer.Concat( take ).Concat( leftover >= 1 ? [0x00] : Array.Empty<byte>() );
                    data = data.Skip( take.Count() ).Skip( leftover >= 1 ? 1 : 0 ).ToArray();

                    len = leftover - (leftover >= 1 ? 1 : 0);

                    if (leftover >= 1) {
                        delimiters++;

                        if (delimiters >= 2) {
                            try {
                                completed = true;
                                delimiters = 0;

                                string typename = "";
                                string packetdata = "";

                                if (buffer.First() == 0xF0) {
                                    ICryptoTransform decryptor = AES.CreateDecryptor();
                                    byte[] decryptedBytes;

                                    var buffer_typename = Encoding.ASCII.GetString(
                                        buffer
                                            .Skip( 1 ).TakeWhile( PendingSplit )
                                            .ToArray()
                                    );

                                    var bytes_typename = Convert.FromBase64String( buffer_typename );

                                    using (var msDecrypt = new MemoryStream( bytes_typename )) {
                                        using (var csDecrypt =
                                               new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read )) {
                                            using (var msPlain = new MemoryStream()) {
                                                csDecrypt.CopyTo( msPlain );
                                                decryptedBytes = msPlain.ToArray();
                                            }
                                        }
                                    }

                                    typename = Encoding.UTF8.GetString( decryptedBytes );

                                    var buffer_packet = Encoding.ASCII.GetString(
                                        buffer
                                            .Skip( 1 ).SkipWhile( PendingSplit )
                                            .Skip( 1 )
                                            .Reverse().Skip( 1 )
                                            .Reverse().TakeWhile( PendingSplit )
                                            .ToArray()
                                    );

                                    var bytes_packet = Convert.FromBase64String( buffer_packet );

                                    using (var msDecrypt = new MemoryStream( bytes_packet )) {
                                        using (var csDecrypt =
                                               new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read )) {
                                            using (var msPlain = new MemoryStream()) {
                                                csDecrypt.CopyTo( msPlain );
                                                decryptedBytes = msPlain.ToArray();
                                            }
                                        }
                                    }

                                    packetdata = Encoding.UTF8.GetString( decryptedBytes );
                                } else if (buffer.First() == 0x0F) {
                                    typename = Encoding.UTF8.GetString(
                                        buffer
                                            .Skip( 1 ).TakeWhile( PendingSplit )
                                            .ToArray()
                                    );

                                    packetdata = Encoding.UTF8.GetString(
                                        buffer
                                            .Skip( 1 ).SkipWhile( PendingSplit )
                                            .Skip( 1 ).TakeWhile( PendingSplit )
                                            .ToArray()
                                    );
                                }

                                buffer = [];

                                if (Multiplayer.Packets.TryGetValue( typename, out var packetType )) {
                                    var packet = JsonSerializer.Deserialize( packetdata, packetType ) as Network.Packet;

                                    if (packet is not null) {
                                        if (Multiplayer.Channels.TryGetValue( packet.Channel, out var channel )) {
                                            channel.Receive( this, packet );
                                        } else {
                                            GD.PrintErr( $"No '{packet.Channel}' channel to receive '{packet}'" );
                                        }
                                    } else {
                                        GD.PrintErr( $"Malformed packet: '{typename}'" );
                                    }
                                } else {
                                    GD.PrintErr( $"Unknown Packet: '{typename}'" );
                                }

                            } catch (Exception e) {
                                GD.PrintErr( $"{e}" );
                            }
                        }


                    }
                } while (data.Length > 0);
            }

            return completed;
        }

        public bool IsOpen => _stream?.Socket?.Connected ?? false;

        public virtual Multiplayer Multiplayer { get; set; }

        public async Task Receive(Network.Packet packet) {
            if (packet?.Channel is null || !Multiplayer.Channels.TryGetValue( packet.Channel, out var channel )) {
                GD.PrintErr( "Null Channel: " + packet.GetType().Name );
                return;
            }

            await channel?.Receive( this, packet );
        }

        public State Status {
            get {
                var ipGlobProp = IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnInfos = ipGlobProp.GetActiveTcpConnections();
                TcpConnectionInformation tcpConnInfo = null;

                tcpConnInfo = tcpConnInfos.FirstOrDefault( conn => {
                        return conn.LocalEndPoint.Port == (_client.Client.LocalEndPoint as IPEndPoint)?.Port &&
                               conn.RemoteEndPoint.Port == (_client.Client.RemoteEndPoint as IPEndPoint)?.Port;
                    }
                );

                if (tcpConnInfo == null)
                    return Connection.State.Idle;

                var tcpState = tcpConnInfo.State;

                switch (tcpState) {
                    case TcpState.Listen:
                    case TcpState.SynSent:
                    case TcpState.SynReceived:
                        return Connection.State.Connecting;

                    case TcpState.Established:
                        return Connection.State.Connected;

                    case TcpState.FinWait1:
                    case TcpState.FinWait2:
                    case TcpState.CloseWait:
                    case TcpState.Closing:
                    case TcpState.LastAck:
                        return Connection.State.Disconnecting;

                    default:
                        return Connection.State.NotReady;
                }
            }
        }

        public virtual IPEndPoint EndPoint { get; set; }

        public Queue<Network.Packet> Pending { get; set; } = new();
    }

    public class Local : Client {
        public Local(Connection.Server server, TcpClient tcpconnection
            ){
            Server = server;
            _client = tcpconnection;
            _timeout = new Timer(TimeSpan.FromSeconds(10));
        
            _timeout.Elapsed += (sender, args) => {
                Server.Disconnect(this);
                _timeout.Enabled = false;
            };
        }

        public Connection.Server Server { get; }

        public override Multiplayer Multiplayer => Server?.Multiplayer;

        public override IPEndPoint? EndPoint => _client.Client.RemoteEndPoint as IPEndPoint;

        Timer _timeout;
    
        public override RSA RSA => Server.RSA;
    
        public override void InterruptTimeout(){
            _timeout.Enabled = false;
            _stream = _client.GetStream();
        }

        protected internal override void Ready() {
            base.Ready();
            
           Multiplayer._clients[EndPoint] = this;
            this.Disconnected += connection => {
                Multiplayer._clients.TryRemove( connection.EndPoint, out _ );
                Multiplayer.Servers[this._client.Client.RemoteEndPoint as IPEndPoint].Disconnect( this );
            };
            
            SendRSAToClient();
        }
        
        void SendRSAToClient() {
            Multiplayer.SystemChannel.Send(
                this,
                new RSAPacket() { PublicKey = RSA.ExportRSAPublicKeyPem() },
                false
            );
        }
    }

    public class Remote : Client {
        public Remote(Multiplayer multiplayer, IPEndPoint endpoint){
            Multiplayer = multiplayer;
            EndPoint = endpoint;
            _client = new TcpClient();
        }

        public override void InterruptTimeout(){
            AES = Aes.Create();
            var key = new byte[16];
            new Random().NextBytes(key);
            var iv = new byte[16];
            new Random().NextBytes(iv);
            AES.Key = key;
            AES.IV = iv;
        }

        public void Connect(){
            _client.Connect(EndPoint);
            if ( Multiplayer.Host is null ) Multiplayer.Host = this;
            base.Ready();
        }
    }

    public enum State {
        NotInitialized,
        NotReady,
        Idle,
        Connecting,
        Connected,
        Disconnecting
    }
    
    public struct Filter
    {
        private IEnumerable<Connection.Client> Connections { get; set; }

        private Predicate<Connection.Client> Predicate { get; set; }

        private FilterType Type { get; set; }

        public Filter(Connection.Filter.FilterType type, Predicate<Connection.Client> predicate)
        {
            // ISSUE: reference to a compiler-generated field
            this.Predicate = predicate;
            this.Type = type;
        }

        public Filter(Connection.Filter.FilterType type, IEnumerable<Connection.Client> connections)
        {
            this.Connections = connections;
            this.Type = type;
        }

        /// <summary>
        /// Is the specified <see cref="T:Sandbox.Connection" /> a valid recipient?
        /// </summary>
        public bool IsRecipient(Connection.Client connection)
        {
            if (this.Type == Connection.Filter.FilterType.Exclude)
            {
                Predicate<Connection.Client> predicate = this.Predicate;
                return predicate == null ? !this.Connections.Contains<Connection.Client>(connection) : !predicate(connection);
            }
            Predicate<Connection.Client> predicate1 = this.Predicate;
            return predicate1 == null ? this.Connections.Contains<Connection.Client>(connection) : predicate1(connection);
        }

        public enum FilterType
        {
            /// <summary>
            /// Only include the connections in the filter when sending a message.
            /// </summary>
            Include,
            /// <summary>
            /// Exclude the connections in the filter when sending a message.
            /// </summary>
            Exclude,
        }
    }
}