using System.Security.Cryptography;
using System.Text;
using SkillQuest.API.Network;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Credentials;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;

using Doohickey = Shared.Game.ECS.Doohickey;

public class Authenticator : Doohickey{
    public override Uri Uri => new Uri("cl://control.skill.quest/users/authenticator");

    public Authenticator(IClientConnection connection){
        _connection = connection;
        _channel = connection.Networker.CreateChannel(Uri);
    }

    IClientConnection _connection;

    IChannel _channel;

    public void Login(string email, string password){
        var authtoken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

        _channel.Send(_connection, new LoginRequestPacket() {
            Email = email,
            AuthToken = authtoken
        });

        _channel.Subscribe<LoginAuthenticationStatusPacket>(OnAuthStatusPacket);
        _channel.Subscribe<LogoutStatusPacket>(OnLogoutStatusPacket);
        _channel.Subscribe<SessionCreateStatusPacket>(OnLoginStatusPacket);
    }

    void OnAuthStatusPacket(IClientConnection sender, LoginAuthenticationStatusPacket packet){
        Console.WriteLine($"Auth Successful: {packet.Success}");

        if (!packet.Success) {
            Console.WriteLine($">  {packet.Reason}");
            AuthenticationFailure?.Invoke( sender, packet.Reason! );
        } else {
            AuthenticationSuccess?.Invoke( sender );
        }
    }

    void OnLoginStatusPacket(IClientConnection sender, SessionCreateStatusPacket packet){
        Console.WriteLine($"Session Created: {packet.Success}");

        if (!packet.Success) {
            Console.WriteLine($">  {packet.Reason}");
            LoginFailure?.Invoke(sender, packet.Reason);
        } else {
            LoginSuccess?.Invoke(sender);
        }
    }

    void OnLogoutStatusPacket(IClientConnection connection, LogoutStatusPacket packet){
        Console.WriteLine($"Logout Successful: {packet.Success}");
        LoggedOut?.Invoke(connection);
    }

    public void Logout(){
        _channel.Send(_connection, new LogoutRequestPacket());
    }

    public delegate void DoAuthenticationSuccess(IClientConnection connection);
    
    public event DoAuthenticationSuccess AuthenticationSuccess;
    
    public delegate void DoAuthenticationFailure(IClientConnection connection, string reason);
    
    public event DoAuthenticationFailure AuthenticationFailure;
    
    public delegate void DoLoginSuccess ( IClientConnection connection );
    
    public event DoLoginSuccess LoginSuccess;
    
    public delegate void DoLoginFailure ( IClientConnection connection, string reason );
    
    public event DoLoginFailure LoginFailure;
    
    public delegate void DoLoggedOut ( IClientConnection connection );
    
    public event DoLoggedOut LoggedOut;
}
