using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

namespace SkillQuest;

public partial class ItemStack : Node {
    private Guid _id = Guid.Empty;

    public Guid ID {
        get {
            return _id;
        }
        set {
            Ledger._stacks.Remove( _id );
            _id = value;
            Ledger._stacks[value] = this;
        }
    }

    [Export] public Item Item { get; set; }
    
    [Export] public long Count { get; set; }

    public static class Ledger {
        internal static Dictionary<Guid, ItemStack> _stacks = new();
        
        public static Dictionary<Guid, ItemStack> Entries => _stacks;
    }
    
    public override void _Ready() {
        
    }
}