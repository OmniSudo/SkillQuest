using Sandbox.Material.Metallurgy;

namespace Sandbox.Item.Mining.Ore;

public class Ore : Core.Item {
    [Property] private Metal Material { get; set; }
}