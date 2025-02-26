using Godot;
using SkillQuest.Actor;

namespace SkillQuest.UI.Login.Select;

public partial class CharacterSelectButton : Container {
    public Character.Info CharacterInfo { get; set; }
    
    public void SetName(string name) {
        this.Name = name;
        GetNode<Button>( "Button" ).SetText( name );
    }

    public string GetName() {
        return this.Name;
    }
}