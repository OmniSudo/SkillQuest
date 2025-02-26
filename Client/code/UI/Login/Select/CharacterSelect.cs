using Godot;
using SkillQuest.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillQuest.UI.Login.Select;

public partial class CharacterSelect : CanvasLayer {
    [Export] public VBoxContainer Selection { get; set; }

    [Export] public Button Create { get; set; }

    [Export] public Button Confirm { get; set; }

    [Export] public Button RotateLeft { get; set; }

    [Export] public Button RotateRight { get; set; }

    [Export] public SubViewport Preview { get; set; }

    [Export] public Node3D PreviewCamera { get; set; }

    [Export] public PanelContainer Stats { get; set; }

    [Export] public Container[] Characters { get; set; }

    private RotateDirection _rotation = RotateDirection.NONE;

    public override void _Ready() {
        Create.Pressed += CreateOnPressed;
        Confirm.Pressed += ConfirmOnPressed;

        RotateLeft.ButtonDown += RotateLeftOnButtonDown;
        RotateLeft.ButtonUp += RotateLeftOnButtonUp;

        RotateRight.ButtonDown += RotateRightOnButtonDown;
        RotateRight.ButtonUp += RotateRightOnButtonUp;

        var character = GD.Load<PackedScene>(
            "Client/scenes/UI/Login/Select/character_button.tscn"
        ).Instantiate<CharacterSelectButton>();
        Selection.AddChild( character );
        character.SetName( "OmniSudo" );
        Characters = [character];
    }

    public override void _Process(double delta) {
        PreviewCamera.Rotation = new Vector3(
            0f,
            PreviewCamera.Rotation.Y + (float)delta * (float)_rotation,
            0f
        );
    }
    
    private void RotateRightOnButtonDown() {
        _rotation = RotateDirection.RIGHT;
    }

    private void RotateRightOnButtonUp() {
        _rotation = RotateDirection.NONE;
    }

    private void RotateLeftOnButtonDown() {
        _rotation = RotateDirection.LEFT;
    }

    private void RotateLeftOnButtonUp() {
        _rotation = RotateDirection.NONE;
    }

    private void CreateOnPressed() {
        throw new System.NotImplementedException();
    }

    private void ConfirmOnPressed() {
        throw new System.NotImplementedException();
    }

    private enum RotateDirection {
        LEFT = -1,
        NONE = 0,
        RIGHT = 1
    }
}