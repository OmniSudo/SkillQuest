using Godot;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SkillQuest;

public partial class Shared : Node {
	public static Node Ledger;
	
	public static Node Items;

	public static Node Materials;

	public static Node ItemStacks;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Ledger = GetNode<Node>("Ledger");
		Items = Ledger.GetNode<Node>("Items");
		Materials = Ledger.GetNode<Node>("Materials" );

		var pick = Item.Ledger.Get( "mining/tool/iron_pickaxe" );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
