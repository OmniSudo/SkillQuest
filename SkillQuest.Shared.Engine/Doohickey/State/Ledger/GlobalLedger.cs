using SkillQuest.Shared.Engine.Doohickey.Item;
using SkillQuest.Shared.Engine.Doohickey.Material.Ledger;

namespace SkillQuest.Shared.Engine.Doohickey.Ledger.State;

public class GlobalLedger{
    public ItemLedger Items { get; set; } = new ItemLedger();
    
    public MaterialLedger Materials { get; set; } = new MaterialLedger();
    
}
