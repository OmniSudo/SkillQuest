using Godot;

namespace SkillQuest.Items.Base.Tools;

public partial class Tool : Item {
    [Export] public string MaterialID {
        get => Material.ID;
        set => Material = Material.Ledger.Get( value );
    }
    
    public Material Material { get; set; }

    [Export] Primary Primary { get; set; }

    public override void _Ready() {
        GD.Print( Material );
    }
}