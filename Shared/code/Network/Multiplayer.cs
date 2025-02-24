using Godot;
using SkillQuest.Packet;
using SkillQuest.Packet.System;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Net.Mime;
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

        SteamAPI.LoadLibrary();
        if (!Steamworks.SteamAPI.Init()) {
            GD.PrintErr( "Failed to initialize Steam API!" );
            return;
        }
    }

    public Side Side { get; set; }

    public Channel SystemChannel { get; set; }

    public ImmutableDictionary<IPEndPoint, Connection.Client> Clients => _clients.ToImmutableDictionary();

    public ImmutableDictionary<IPEndPoint, Connection.Server> Servers => _servers.ToImmutableDictionary();

    public ImmutableDictionary<string, Type> Packets => _packets.ToImmutableDictionary();

    public ImmutableDictionary<string, Channel> Channels => _channels.ToImmutableDictionary();

    private ConcurrentDictionary<IPEndPoint, Connection.Client> _clients = new();

    private ConcurrentDictionary<IPEndPoint, Connection.Server> _servers = new();

    private ConcurrentDictionary<string, Channel> _channels = new();

    private ConcurrentDictionary<string, Type> _packets = new();

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

    public void Update(double delta) {
        foreach (var (endpoint, client) in _clients) {
            try {
                client.Receive();
            } catch (Exception e) {
                GD.PrintErr( $"Unable to process connection {e}" );
                if (client.IsOpen) client.Disconnect();
            }
        }

        foreach (var (endpoint, server) in _servers) {
            server.Update( delta );
        }
    }

    public Task<Connection.Client?> Connect(IPEndPoint endpoint) {
        var client = new Connection.Remote( this, endpoint );

        TaskCompletionSource<Connection.Client> tcs = new();

        client.Connected += (connection) => {
            if (!tcs.Task.IsCompleted) tcs.SetResult( connection );
        };

        client.Disconnected += (connection) => {
            _clients.TryRemove( connection.EndPoint, out _ );
        };

        _clients[endpoint] = client;

        client.Connect();

        return tcs.Task;
    }

    public Connection.Server? Host(short port) {
        Steamworks.GameServer.Init( 0, 3698, 8963, EServerMode.eServerModeAuthenticationAndSecure, "v0.0.0" );

        var server = new Connection.Server( this, port );

        _servers[server.EndPoint] = server;

        server.Connected += (server, client) => {
            _clients[client.EndPoint] = client;
            client.Disconnected += connection => {
                _clients.TryRemove( connection.EndPoint, out _ );
                server.Disconnect( connection );
            };
        };

        return server;
    }

    protected internal void OnRSAPacket(Connection.Client sender, RSAPacket packet) {
        sender.InterruptTimeout();
        sender.RSA.ImportFromPem( packet.PublicKey );
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
            (sender as Connection.Remote).OnConnected();
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
            sender.AES = Aes.Create();
            sender.AES.Key = Convert.FromBase64String( split[0] );
            sender.AES.IV = Convert.FromBase64String( split[1] );
        } catch (Exception e) {
            GD.PrintErr( $"Failed to initialize secure connection with client...{e}" );
        }
    }

    protected internal void OnSteamAuthPacket(Connection.Client sender, SteamAuthPacket packet) {
        try {
            var result = SteamGameServer.BeginAuthSession(
                packet.Token, packet.Token.Length,
                new CSteamID( packet.SteamId )
            );

            if (result == EBeginAuthSessionResult.k_EBeginAuthSessionResultOK) {
                sender.SteamId = packet.SteamId;
                GD.Print( $"Steam user {sender.Username} logged in." );
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