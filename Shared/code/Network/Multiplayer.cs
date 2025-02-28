using Godot;
using SkillQuest.Packet;
using SkillQuest.Packet.System;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkillQuest.Network;

public partial class Multiplayer {
    public Multiplayer() {
        SystemChannel = CreateChannel( new Uri( "packet://system.skill.quest/" ) );
        SystemChannel.Subscribe<RSAPacket>( OnRSAPacket );
        SystemChannel.Subscribe<AESPacket>( OnAESPacket );
        SystemChannel.Subscribe<SteamAuthPacket>( OnSteamAuthPacket );
        
        SystemChannel.Subscribe<RpcPacket>( RpcAttribute.OnRpcPacket );
        
        SteamAPI.LoadLibrary();
        if (!Steamworks.SteamAPI.Init()) {
            GD.PrintErr( "Failed to initialize Steam API!" );
            return;
        }
    }
    
    public static Connection.Client? Host { get; protected internal set; }
    
    public Channel SystemChannel { get; set; }

    public ImmutableDictionary<IPEndPoint, Connection.Client> Clients => _clients.ToImmutableDictionary();

    public ImmutableDictionary<IPEndPoint, Connection.Server> Servers => _servers.ToImmutableDictionary();

    public ImmutableDictionary<string, Type> Packets => _packets.ToImmutableDictionary();

    public ImmutableDictionary<string, Channel> Channels => _channels.ToImmutableDictionary();

    protected internal ConcurrentDictionary<IPEndPoint, Connection.Client> _clients = new();

    protected internal ConcurrentDictionary<IPEndPoint, Connection.Server> _servers = new();

    protected internal ConcurrentDictionary<string, Channel> _channels = new();

    protected internal ConcurrentDictionary<string, Type> _packets = new();

    public Channel CreateChannel(Uri uri) {
        var name = new UriBuilder( uri ) { Scheme = "packet" }.Uri.ToString();

        if (_channels.TryGetValue( name, out Channel? value )) {
            return value;
        }

        value = new Channel( this, name );
        _channels[name] = value;

        return value;
    }

    public void DestroyChannel(Channel channel) {
        _channels.TryRemove( channel.Name, out _ );
    }
    
    public Task<Connection.Client?> Connect(IPEndPoint endpoint) {

        var client = new Connection.Remote( this, endpoint );
        Multiplayer.Host ??= client;
        
        TaskCompletionSource<Connection.Client> tcs = new();

        client.Connected += (connection) => {
            if (!tcs.Task.IsCompleted) tcs.SetResult( connection );
        };

        client.Disconnected += (connection) => {
            if ( Multiplayer.Host == connection ) Multiplayer.Host = _clients.FirstOrDefault().Value;

            _clients.TryRemove( connection.EndPoint, out _ );
        };

        _clients[endpoint] = client;

        client.Connect();

        return tcs.Task;
    }

    public Connection.Server? Bind(short port) {
        Steamworks.GameServer.Init( 0, 3698, 8963, EServerMode.eServerModeAuthenticationAndSecure, "v0.0.0" );

        var server = new Connection.Server( this, port );

        _servers[server.EndPoint] = server;
        
        return server;
    }

    protected internal void OnRSAPacket(Connection.Client sender, RSAPacket packet) {
        sender.InterruptTimeout();
        sender.SteamId = SteamUser.GetSteamID().m_SteamID;
        sender.RSA.ImportFromPem( packet.PublicKey );
        sender.AES = Aes.Create();
        var key = Convert.ToBase64String( sender.AES.Key );
        var iv = Convert.ToBase64String( sender.AES.IV );
        var plaintext = key + (char)0x00 + iv;

        try {
            byte[] encryptedData = sender.RSA.Encrypt( Encoding.ASCII.GetBytes( plaintext ), RSAEncryptionPadding.Pkcs1 );
            SystemChannel.Send( sender, new AESPacket() { Data = Convert.ToBase64String( encryptedData ) }, false ).Wait();

            var bytes = new byte[1024];
            uint size;
            var identity = new SteamNetworkingIdentity() { };

            SteamUser.GetAuthSessionTicket( bytes, 1024, out size, ref identity );

            SystemChannel.Send( sender, new SteamAuthPacket() {
                SteamId = SteamUser.GetSteamID().m_SteamID,
                Token = bytes,
            }, true ).Wait();
            sender.OnConnected();
        } catch (Exception e) {
            sender.Disconnect();
            GD.PrintErr( $"Failed to initialize secure connection with server...{e}" );
        }
    }

    protected internal void OnAESPacket(Connection.Client sender, AESPacket packet) {
        try {
            var plaintext =
                Encoding.ASCII.GetString( sender.RSA.Decrypt( Convert.FromBase64String( packet.Data ),
                    RSAEncryptionPadding.Pkcs1 ) );
            var split = plaintext.Split( (char)0x00 );
            var aes = Aes.Create();
            aes.Key = Convert.FromBase64String( split[0] );
            aes.IV = Convert.FromBase64String( split[1] );
            sender.AES = aes;
        } catch (Exception e) {
            GD.PrintErr( $"Failed to initialize secure connection with client...{e}" );
        }
    }

    protected internal void OnSteamAuthPacket(Connection.Client sender, SteamAuthPacket packet) {
        try {
            var status = _clients.Where( pair => pair.Value.SteamId == packet.SteamId ).Select( pair => pair.Value )
                .FirstOrDefault()?.Status ?? Connection.State.Disconnecting;

            if ( status != Connection.State.Connected && status != Connection.State.Connecting) {
                SteamGameServer.EndAuthSession( new CSteamID( packet.SteamId ) );
            }
            
            var result = SteamGameServer.BeginAuthSession(
                packet.Token, packet.Token.Length,
                new CSteamID( packet.SteamId )
            );

            if (result == EBeginAuthSessionResult.k_EBeginAuthSessionResultOK) {
                sender.SteamId = packet.SteamId;
                GD.Print( $"Steam user {sender.Username} logged in." );
                sender.OnConnected();
                (sender as Connection.Local).Server.OnConnected( sender );
                sender.Disconnected += connection => {
                    SteamGameServer.EndAuthSession( new CSteamID( packet.SteamId ) );
                };
            } else {
                sender.Disconnect(); // TODO: Error instead of just disconnecting
            }
        } catch (Exception e) {
            GD.PrintErr( $"Failed to auth user with SteamId {packet.SteamId}" );
            GD.PrintErr( e.Message );
            GD.PrintErr( e.StackTrace );
        }
    }

    private Callback<ValidateAuthTicketResponse_t> _authCallback;

    private ConcurrentDictionary<CSteamID, Connection.Client> _steamids = new();
    private ConcurrentDictionary<CSteamID, TaskCompletionSource<Connection.Client>> _sendTasks = new();

    public void AddPacket<TPacket>() where TPacket : Network.Packet {
        var type = typeof(TPacket);
        _packets[type.FullName!] = type;
    }
}