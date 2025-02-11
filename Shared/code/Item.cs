using Godot;
using System;

namespace SkillQuest;

public partial class Item : Node {
	[Export] public string Singular { get; set; } = "";
	
	[Export] public string Plural { get; set; } = "";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}
}
