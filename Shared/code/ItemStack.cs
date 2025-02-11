using Godot;

namespace SkillQuest;

public partial class ItemStack : Node {
    [Export] public Item Item { get; set; }
    
    [Export] public long Count { get; set; }
    
    public override void _Ready() {
        
    }
}