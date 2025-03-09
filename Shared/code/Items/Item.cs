using Godot;
using System;
using System.Collections.Generic;

namespace SkillQuest;

public partial class Item : Node {
    public string ID => _id;

    [Export] public string Singular { get; set; } = "";

    [Export] public string Plural { get; set; } = "";

    private string _id;

    public static class Ledger {
        static Dictionary<string, Item> _items = new();

        public static Dictionary<string, Item> Entries => _items;

        public static Item Get(string id) {
            if (_items.ContainsKey( id )) {
                return _items[id];
            }

            return Load( id );
        }

        public static Item Load(string id, Item? item = null) {
            item = _items[id] = item ??
                                GD.Load<PackedScene>(
                                    "res://Shared/assets/items/" + id + ".tscn"
                                ).Instantiate<Item>();

            var split = id.Split( '/' );

            var root = Shared.SH.Items;
            var node = root;

            foreach (var name in split[..^1]) {
                node.AddChild( node = new Node() { Name = name } );
            }

            item._id = id;
            item.Name = split[^1];
            node.AddChild( item );

            GD.Print( $"Loaded {item.Name} @ {item.ID}" );

            return item;
        }
    }
}