using Godot;
using System;
using Godot.Collections;
using Godot.NativeInterop;

namespace SkillQuest;

public partial class Material : Node {
    [Export] public string Name { get; set; }

    public string ID => _id;

    string _id;

    public static class Ledger {
        private static Dictionary<String, Material> _entries = new();

        public static Dictionary<String, Material> Entries => _entries;

        public static Material Get(string id) {
            if (_entries.ContainsKey( id )) {
                return _entries[id];
            }

            return Load( id );
        }

        public static Material Load(string id, Material? material = null) {
            material = _entries[id] = material ??
                                GD.Load<PackedScene>(
                                    "res://Shared/assets/materials/" + id + ".tscn"
                                ).Instantiate<Material>();

            var split = id.Split( '/' );

            var root = Shared.Materials;
            var node = root;

            foreach (var name in split[..^1]) {
                node.AddChild( node = new Node() { Name = name } );
            }

            material._id = id;
            material.Name = split[^1];
            node.AddChild( material );

            GD.Print( $"Loaded {material.Name} @ {material.ID}" );

            return material;
        }
    }
}