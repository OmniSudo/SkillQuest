using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.CharacterSelect;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Login;

using Doohickey = Shared.Game.ECS.Doohickey;

public class GuiLoginSignup : Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri( "gui://skill.quest/login" );

    private void OpenCharacterSelect(IClientConnection connection){
        Stuff?.Remove(this);
        _ = new GuiCharacterSelection(_connection)?.Render();
    }
    
    private void OnRenderReset(IClientConnection clientConnection, string reason) => _ = Render();

    public GuiLoginSignup(IClientConnection connection){
        _connection = connection;
        
        Stuffed += (_, _) => {
            Authenticator.Instance.AuthenticationFailure += OnRenderReset;

            Authenticator.Instance.LoginFailure += OnRenderReset;

            Authenticator.Instance.LoginSuccess += OpenCharacterSelect;

            Authenticator.Instance.LoggedOut += OnRenderReset;
        };

        Unstuffed += (_, _) => {
            Authenticator.Instance.AuthenticationFailure -= OnRenderReset;

            Authenticator.Instance.LoginFailure -= OnRenderReset;

            Authenticator.Instance.LoggedOut -= OnRenderReset;

            Authenticator.Instance.LoginSuccess -= OpenCharacterSelect;
        };
    }

    private IClientConnection _connection { get; set; }
    
    public async Task Render(){
        await Task.Run(() => {
            get_email:
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
                goto get_email;
            }

            Console.WriteLine();

            try {
                var addr = new System.Net.Mail.MailAddress(trimmed);

                if (addr.Address != trimmed) {
                    Console.WriteLine("Invalid Email");
                    goto get_email;
                }
            } catch {
                Console.WriteLine("Invalid Email");
                goto get_email;
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

            Authenticator.Instance.Login( _connection, email, pass);
        });
    }
}
