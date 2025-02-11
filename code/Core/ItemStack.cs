using Godot;

namespace SkillQuest.Core;

public partial class ItemStack : Node {
    [Export] public Item Item { get; set; }
    
    [Export] public long Count { get; set; }
    
    public override void _Ready() {
        
    }
}