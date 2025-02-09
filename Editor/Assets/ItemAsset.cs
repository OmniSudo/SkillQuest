namespace Sandbox.Assets;

[GameResource("Item", "item", "An Item", Category = "SkillQuest")]
public partial class ItemAsset : GameResource {
    public string Name { get; set; }
}