using Godot;
using SkillQuest.Actor;
using SkillQuest.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

    private Character.Info? _selected;

    public override void _Ready() {
        Create.Pressed += CreateOnPressed;
        Confirm.Pressed += ConfirmOnPressed;

        RotateLeft.ButtonDown += RotateLeftOnButtonDown;
        RotateLeft.ButtonUp += RotateLeftOnButtonUp;

        RotateRight.ButtonDown += RotateRightOnButtonDown;
        RotateRight.ButtonUp += RotateRightOnButtonUp;
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
        if (_selected is not null) Select?.Invoke( _selected );
    }

    private enum RotateDirection {
        LEFT = -1,
        NONE = 0,
        RIGHT = 1
    }

    public static class Client {
        private static TaskCompletionSource<Character.Info> _select;

        public static Task<Character.Info> Open(Character.Info[] characters) {
            var ui = GD.Load<PackedScene>(
                "Client/scenes/UI/Login/Select/character_select.tscn"
            ).Instantiate<CharacterSelect>();
            ui.SetCharacters( characters );
            Shared.SH.CallDeferred( () => {
                    SkillQuest.Client.UI.AddChild( ui );
                }
            );

            _select = new TaskCompletionSource<Character.Info>();
            ui.Select += UiOnSelect;

            void UiOnSelect(Character.Info info) {
                _select.SetResult( info );
                ui.Select -= UiOnSelect;
                Close();
            }

            return _select.Task;
        }

        public static void Close() {
            var node = SkillQuest.Client.UI.GetNode( "Character Select" );
            SkillQuest.Client.UI.RemoveChild( node );
        }
    }

    private void SetCharacters(Character.Info[] characters) {
        var scene = GD.Load<PackedScene>(
            "Client/scenes/UI/Login/Select/character_button.tscn"
        );

        var containers = new List<Container>();
        foreach (var character in characters) {
            var element = scene.Instantiate<CharacterSelectButton>();
            Selection.AddChild( element );
            element.SetName( character.Name );
            element.CharacterInfo = character;
            var button = element.GetNode<Button>( "Button" );
            button.Pressed += () => {
                _selected = element.CharacterInfo;
            };
            containers.Add( element );
        }

        Characters = containers.ToArray();
    }

    private delegate void OnSelect(Character.Info info);

    private event OnSelect Select;

    public static class Server {
        public static Task<Character.Info> GetSelection(Connection.Client client) {
            var guid = Guid.NewGuid();
            _getCharacter[client] = new TaskCompletionSource<Character.Info>();

            using (Network.Rpc.FilterInclude( client )) {
                _CL_Open();
            }

            return _getCharacter[client].Task;
        }

        public static void CloseOn(Connection.Client client) {
            using (Network.Rpc.FilterInclude( client )) {
                _CL_Close();
            }
        }
    }

    private static Dictionary<Connection.Client, TaskCompletionSource<Character.Info>> _getCharacter = new();

    [Broadcast]
    private static async void _CL_Open() {
        var sender = Network.Rpc.Caller;
        Character.Info[] characters = [
            new Character.Info() {
                Name = sender.Username,
            }
        ];
        var selection = await Client.Open( characters );
        _SV_Get( selection );
    }

    [Host]
    private static async void _SV_Get(Character.Info info) {
        if (_getCharacter.Remove( Network.Rpc.Caller, out var tcs )) {
            tcs.SetResult( info );
        }
    }

    [Broadcast]
    private static async void _CL_Close() {
        Client.Close();
    }
}