using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SkillQuest.API.Network;

namespace SkillQuest.Shared.Engine.Network;

class RemoteClientConnection : ClientConnection, IRemoteConnection{
    public INetworker Networker { get; }

    public IPEndPoint EndPoint { get; }

    public bool Running { get; set; } = false;

    public RemoteClientConnection(INetworker networker, IPEndPoint endpoint){
        Networker = networker;
        EndPoint = endpoint;
        Connection = new TcpClient();
    }

    public void InterruptTimeout(){
        AES = Aes.Create();
        var key = new byte[16];
        new Random().NextBytes(key);
        var iv = new byte[16];
        new Random().NextBytes(iv);
        AES.Key = key;
        AES.IV = iv;
    }

    public void Disconnect(){
        OnDisconnect();
        Connection.Close();
    }

    public event IClientConnection.DoConnect? Connected;

    public event IClientConnection.DoDisconnect? Disconnected;


    public void Connect(){
        Connection.Connect(EndPoint);
        Console.WriteLine($"Connected to {EndPoint}");
        _stream = Connection.GetStream();
        Listen();
    }

    protected internal void OnConnected(){
        Connected?.Invoke(this);
        Console.WriteLine($"Connected @ {EndPoint}");
    }

    protected internal void OnDisconnect(){
        Disconnected?.Invoke(this);
        Console.WriteLine($"Disconnected @ {EndPoint}");
    }
}
