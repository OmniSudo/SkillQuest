using Godot;
using SkillQuest.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SkillQuest;

public partial class Shared : Node {
    public const string VERSION = "0.0.0";
    
    
    public static Node Ledger;

    public static Node Items;

    public static Node Materials;

    public static Node ItemStacks;

    public static Node System;

    public static Multiplayer Multiplayer;

    public static Shared SH;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        SH = this;
        
        Ledger = GetNode<Node>( "Ledger" );
        Items = Ledger.GetNode<Node>( "Items" );
        Materials = Ledger.GetNode<Node>( "Materials" );
        System = GetNode<Node>( "System" );
        Multiplayer = new Multiplayer();
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