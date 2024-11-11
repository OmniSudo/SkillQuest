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

    public async Task DoLogin(){
        getemail:
        Console.Write("email > ");
        var email = string.Empty;
        ConsoleKey key;

        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && email.Length > 0) {
                Console.Write("\b \b");
                email = email[0..^1];
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                Console.Write(keyInfo.KeyChar);
                email += keyInfo.KeyChar;
            }
        } while ( key != ConsoleKey.Enter );

        var trimmed = email.Trim();

        if (trimmed.EndsWith(".")) {
            Console.WriteLine("Invalid Email");
            goto getemail;
        }
        
        Console.WriteLine();

        try {
            var addr = new System.Net.Mail.MailAddress(trimmed);

            if (addr.Address != trimmed) {
                Console.WriteLine("Invalid Email");
                goto getemail;
            }
        } catch {
            Console.WriteLine("Invalid Email");
            goto getemail;
        }

        email = trimmed;

        Console.Write("password > ");
        var pass = string.Empty;

        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0) {
                Console.Write("\b \b");
                pass = pass[0..^1];
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while ( key != ConsoleKey.Enter );
        Console.WriteLine();
        
        Login(email, pass);
    }

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
            DoLogin();
        }
    }

    void OnLoginStatusPacket(IClientConnection sender, SessionCreateStatusPacket packet){
        Console.WriteLine($"Session Created: {packet.Success}");

        if (!packet.Success) {
            Console.WriteLine($">  {packet.Reason}");
            DoLogin();
        }
    }

    void OnLogoutStatusPacket(IClientConnection connection, LogoutStatusPacket packet){
        Console.WriteLine($"Logout Successful: {packet.Success}");
    }

    public void Logout(){
        _channel.Send(_connection, new LogoutRequestPacket());
    }
}
