// entire file content ...
public class DirNode {
    public string Name { get; set; }
    public DirNode Parent { get; set; }
    public Dictionary<string, DirNode> Children { get; set; }
    public IThing Thing { get; set; }

    public bool Opened { get; set; } = false;
}
