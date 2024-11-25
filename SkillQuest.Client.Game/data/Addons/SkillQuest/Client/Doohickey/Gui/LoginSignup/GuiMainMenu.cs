using System.Net;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;

public class GuiMainMenu : Shared.Engine.ECS.Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri("ui://skill.quest/mainmenu");

    string address = "127.0.0.1:3698";
    string email = "omni@skill.quest";
    string password = "";

    IClientConnection? connection = null;

    public GuiMainMenu(){
        Stuffed += (_, _) => {
            Authenticator.Instance.LoginSuccess += OpenCharacterSelect;
        };

        Unstuffed += (_, _) => {
            Authenticator.Instance.LoginSuccess -= OpenCharacterSelect;
        };
    }

    void OpenCharacterSelect(IClientConnection clientConnection){
        if (connection is null) return;
        
        Stuff?.Remove(this);
        Stuff?.Add( new GuiCharacterSelection(connection) );
    }

    Task? _connect = null;

    public void Render(){
        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings
            )
        ) {
            ImGui.InputTextWithHint( "Address", "address", ref address, 128 );
            ImGui.InputTextWithHint( "Email", "email", ref email, 128 );
            ImGui.InputTextWithHint( "Password", "password", ref password, 128, ImGuiInputTextFlags.Password );
            
            ImGui.Separator();
            if (
                ImGui.Button("Login")
            ) {
                _connect = Task.Run(async () => {
                    var trimmed = email.Trim();

                    if (trimmed.EndsWith(".")) {
                        Console.WriteLine("Invalid Email");
                        return;
                    }
                    
                    try {
                        var addr = new System.Net.Mail.MailAddress(trimmed);

                        if (addr.Address != trimmed) {
                            Console.WriteLine("Invalid Email");
                            return;
                        }
                    } catch {
                        Console.WriteLine("Invalid Email");
                        return;
                    }

                    email = trimmed;
                    connection = await SH.Net.Connect(IPEndPoint.Parse(address));
                    
                    Authenticator.Instance.Login(connection, email, password);
                });
            }
            ImGui.End();
        }
    }
}
