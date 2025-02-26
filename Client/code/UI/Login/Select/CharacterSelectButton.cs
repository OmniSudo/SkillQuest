using Godot;

namespace SkillQuest.UI.Login.Select;

public partial class CharacterSelectButton : Container {
    public void SetName(string name) {
        this.Name = name;
        GetNode<Button>( "Button" ).SetText( name );
    }

    public string GetName() {
        return this.Name;
    }
}