using Godot;
using System;
using Godot.Collections;

namespace SkillQuest;

public partial class Material : Node
{
	[Export] public string Name { get; set; }

	public static class Ledger {
		public static Dictionary<String, Material> _data = new();
		
		public static Material? Get( string id) {
			if (!_data.ContainsKey(id)) {
				
			}
			
			return null;
		}
	}
}
