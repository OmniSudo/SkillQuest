namespace Sandbox.Core;

public class ItemStack : Component {
    [Property] public Item Item { get; set; }
    
    [Property] public long Count { get; set; }
}