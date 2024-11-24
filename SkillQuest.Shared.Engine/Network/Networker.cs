using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

public sealed class Networker : INetworker{

    public ImmutableDictionary<IPEndPoint, IClientConnection> Clients => _clients.ToImmutableDictionary();

    public ImmutableDictionary<IPEndPoint, IServerConnection> Servers => _servers.ToImmutableDictionary();

    public ImmutableDictionary<string, IChannel> Channels => _channels.ToImmutableDictionary();

    private ConcurrentDictionary<IPEndPoint, IClientConnection> _clients = new();
    private ConcurrentDictionary<IPEndPoint, IServerConnection> _servers = new();
    private ConcurrentDictionary<string, IChannel> _channels = new();

    public Networker(){
        _systemChannel = CreateChannel(new Uri("packet://skill.quest/system"));
        _systemChannel.Subscribe<RSAPacket>(OnRSAPacket);
        _systemChannel.Subscribe<AESPacket>(OnAESPacket);
    }

    public Task<IClientConnection?> Connect(IPEndPoint endpoint){
        var client = new RemoteConnection(this, endpoint);
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

    public async Task<IServerConnection?> Host(short port){
        var server = new ServerConnection(this, port);
        await server.Listen();
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

    protected internal IChannel _systemChannel;

    protected internal void OnRSAPacket(IClientConnection sender, RSAPacket packet){
        sender.InterruptTimeout();
        sender.RSA.ImportFromPem(packet.PublicKey);
        var key = Convert.ToBase64String(sender.AES.Key );
        var iv = Convert.ToBase64String(sender.AES.IV);
        var plaintext = key + (char)0x00 + iv;
        
        try {
            byte[] encryptedData = sender.RSA.Encrypt(Encoding.ASCII.GetBytes(plaintext), RSAEncryptionPadding.Pkcs1);
            _systemChannel.Send( sender, new AESPacket() { Data = Convert.ToBase64String( encryptedData ) } );
            (sender as RemoteConnection).OnConnected();
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
            ((sender as LocalConnection).Server as ServerConnection).OnConnected(sender);
        } catch (Exception e) {
            Console.WriteLine( $"Failed to initialize secure connection with client...{e}" );
        }
    }
}
