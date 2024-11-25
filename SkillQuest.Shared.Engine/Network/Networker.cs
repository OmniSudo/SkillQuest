using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.Network.Packet;

namespace SkillQuest.Shared.Engine.Network;

public sealed class Networker : INetworker{

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    public ImmutableDictionary<IPEndPoint, IServerConnection> Servers => _servers.ToImmutableDictionary();
    
    public ImmutableDictionary<string, Type> Packets => _packets.ToImmutableDictionary();

    public ImmutableDictionary<string, IChannel> Channels => _channels.ToImmutableDictionary();

    private ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();
    private ConcurrentDictionary<IPEndPoint, IServerConnection> _servers = new();
    private ConcurrentDictionary<string, IChannel> _channels = new();
    ConcurrentDictionary<string, Type> _packets = new();

    public Networker(){
        SystemChannel = CreateChannel(new Uri("packet://skill.quest/system"));
        LoadPacketsFromAssembly( this.GetType().Assembly );
        SystemChannel.Subscribe<RSAPacket>(OnRSAPacket);
        SystemChannel.Subscribe<AESPacket>(OnAESPacket);
    }

    public Task<IClientConnection?> Connect(IPEndPoint endpoint){
        var client = new RemoteClientConnection(this, endpoint);
        _clients.TryAdd(endpoint, client);

        TaskCompletionSource<IClientConnection> tcs = new();

        client.Connected += (connection) => { tcs.SetResult(connection); };

        client.Disconnected += (connection) => {
            _clients.TryRemove(connection.EndPoint, out _);
        };

        client.Connect();

        return tcs.Task;
    }

    public void Listen(ILocalConnection connection){
        _clients[connection.EndPoint] = connection;
    }

    public IServerConnection? Host(short port){
        var server = new ServerConnection(this, port);
        server.Listen();
        return server;
    }

    public IChannel CreateChannel(Uri uri){
        var name = new UriBuilder(uri) { Scheme = "packet" }.Uri.ToString();

        if (!_channels.ContainsKey(name))
            _channels[name] = new Channel() { Name = name };

        return _channels[name];
    }

    public void DestroyChannel(IChannel channel){
        _channels.TryRemove(channel.Name, out _);
    }

    public IChannel SystemChannel { get; }

    protected internal void OnRSAPacket(IClientConnection sender, RSAPacket packet){
        sender.InterruptTimeout();
        sender.RSA.ImportFromPem(packet.PublicKey);
        var key = Convert.ToBase64String(sender.AES.Key );
        var iv = Convert.ToBase64String(sender.AES.IV);
        var plaintext = key + (char)0x00 + iv;
        
        try {
            byte[] encryptedData = sender.RSA.Encrypt(Encoding.ASCII.GetBytes(plaintext), RSAEncryptionPadding.Pkcs1);
            SystemChannel.Send( sender, new AESPacket() { Data = Convert.ToBase64String( encryptedData ) } );
            (sender as RemoteClientConnection).OnConnected();
        }
        catch (Exception e) {
            sender.Disconnect();
            Console.WriteLine( $"Failed to initialize secure connection with server...{e}");
        }
    }

    protected internal void OnAESPacket(IClientConnection sender, AESPacket packet){
        try {
            var plaintext = Encoding.ASCII.GetString( sender.RSA.Decrypt(Convert.FromBase64String(packet.Data), RSAEncryptionPadding.Pkcs1));
            var split = plaintext.Split((char)0x00);
            sender.AES = Aes.Create();
            sender.AES.Key = Convert.FromBase64String(split[0]);
            sender.AES.IV = Convert.FromBase64String(split[1]);
            ((sender as LocalClientConnection).Server as ServerConnection).OnConnected(sender);
        } catch (Exception e) {
            Console.WriteLine( $"Failed to initialize secure connection with client...{e}" );
        }
    }

    public void LoadPacketsFromAssembly(Assembly assembly){
        foreach (var type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(API.Network.Packet)))) {
            _packets[type.FullName] = type;
        }
    }
}
