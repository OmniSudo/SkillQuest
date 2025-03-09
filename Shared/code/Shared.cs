using Godot;
using SkillQuest.Network;
using SkillQuest.Terrain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SkillQuest;

public partial class Shared : Node {
    public const string VERSION = "0.0.0";
    
    public Node Ledger;

    public Node Items;

    public Node Materials;

    public Node ItemStacks;

    public Node System;

    [Export] public World World;

    public Multiplayer Multiplayer;

    public static Shared SH;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        SH = this;

        System = GetNode<Node>( "System" );
        Multiplayer = new Multiplayer();

        World ??= GetNodeOrNull<World>( "World" ) ?? new World() {
            Name = "World",
        };
        
        Ledger = GetNode<Node>( "Ledger" );
        Items = Ledger.GetNode<Node>( "Items" );
        Materials = Ledger.GetNode<Node>( "Materials" );
    }

    public override void _Process(double delta) {
        while (_defferedActions.TryDequeue( out var action )) {
            action?.Invoke();
        }
    }

    private ConcurrentQueue<Action> _defferedActions = new();
    
    public void CallDeferred(Action action) {
        _defferedActions.Enqueue( action );
    }
}