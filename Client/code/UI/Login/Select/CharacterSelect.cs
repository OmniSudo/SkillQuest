using Godot;

namespace SkillQuest.UI.Login.Select;

public partial class CharacterSelect : CanvasLayer {
    [Export] public VBoxContainer Characters { get; set; }
    
    [Export] public Button Create { get; set; }
    
    [Export] public Button Confirm { get; set; }
    
    [Export] public Button RotateLeft { get; set; }
    
    [Export] public Button RotateRight { get; set; }
    
    [Export] public SubViewport Preview { get; set; }
    
    [Export] public Node3D PreviewCamera { get; set; }

    [Export] public PanelContainer Stats { get; set; }

    private RotateDirection _rotation = RotateDirection.NONE;

    public override void _Ready() {
        Create.Pressed += CreateOnPressed;
        Confirm.Pressed += ConfirmOnPressed;
        
        RotateLeft.ButtonDown += RotateLeftOnButtonDown;
        RotateLeft.ButtonUp += RotateLeftOnButtonUp;

        RotateRight.ButtonDown += RotateRightOnButtonDown;
        RotateRight.ButtonUp += RotateRightOnButtonUp;
    }

    public override void _Process(double delta) {
        PreviewCamera.Rotation = new Vector3( 0f, PreviewCamera.Rotation.Y + ( float ) delta * (_rotation == RotateDirection.LEFT ? -1.0f : 0.0f ), 0f );
        PreviewCamera.Rotation = new Vector3( 0f, PreviewCamera.Rotation.Y + ( float ) delta * (_rotation == RotateDirection.RIGHT ? 1.0f : 0.0f ), 0f );
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