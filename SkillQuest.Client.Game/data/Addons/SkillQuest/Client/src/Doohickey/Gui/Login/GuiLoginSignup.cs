using System.Net;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.CharacterSelect;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Users;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.Login;

using Doohickey = Shared.Game.ECS.Doohickey;
using static Shared.Game.State;

public class GuiLoginSignup : Doohickey, IRenderable{
    public override Uri? Uri { get; } = new Uri( "gui://skill.quest/login" );

    private void OpenCharacterSelect(IClientConnection connection){
        Stuff?.Remove(this);
        _ = Stuff?.Add( new GuiCharacterSelection(_connection) ).Render();
    }

    private IClientConnection? _connection { get; set; }

    private string _serveraddress = "127.0.0.1:3698";
    private string _username = "root@skill.quest";
    private string _password = "";
    
    public void Render(){
        ImGui.Begin("Login");
        
        ImGui.InputText( "address", ref _serveraddress, 256 );
        ImGui.InputText( "username", ref _username, 256 );
        ImGui.InputText( "password", ref _password, 256, ImGuiInputTextFlags.Password );

        if (ImGui.Button("Login")){
            if (_connection is null){
                Task.Run(() => {
                    _connection = SH.Net.Connect(IPEndPoint.Parse(_serveraddress)).Result;

                    Authenticator.Instance.AuthenticationSuccess += InstanceOnAuthenticationSuccess;
                    Authenticator.Instance.Login(_connection, _username, _password);
                });
            }
        }
        
        ImGui.End();
    }

    private void InstanceOnAuthenticationSuccess(IClientConnection connection){
        throw new NotImplementedException();
    }
}
