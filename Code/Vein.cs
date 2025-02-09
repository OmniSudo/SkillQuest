
public sealed class Vein : Component
{
	[Property] public string Name { get; set; }
	
	[Property] public long Level { get; set; }
	
	[Property] public long ExperienceDrop { get; set; }
	
	protected override void OnUpdate()
	{
	}
}
