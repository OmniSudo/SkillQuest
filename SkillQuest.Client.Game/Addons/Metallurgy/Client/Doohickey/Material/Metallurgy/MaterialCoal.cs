namespace SkillQuest.Client.Game.Addons.Metallurgy.Client.Doohickey.Material.Metallurgy;

public class MaterialCoal : Shared.Engine.Thing.Material {
    public override Uri? Uri { get; set; } = new Uri("material://skill.quest/metallurgy/coal");
    
    public override string Name => "Coal";
}
