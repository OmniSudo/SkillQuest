using Godot;
using SkillQuest.UI.Login.Select;
using System;

public partial class Ui : Control {
    public override void _Ready() {
    }

    public void OpenCharacterSelect() {
        AddChild(
            GD.Load<PackedScene>(
                "Client/scenes/UI/Login/Select/character_select.tscn"
            ).Instantiate<CharacterSelect>()
        );
    }
}