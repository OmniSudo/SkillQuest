using SkillQuest.World;

namespace SkillQuest.Procedural.Node;

public partial class EntryPointNode : Godot.Node {
    public virtual bool Main(Region region) {
        return false;
    }

    public delegate void DoGenerate(Region region);

    public event DoGenerate Generate;

    protected virtual void OnGenerate(Region region) {
        Generate?.Invoke(region);
    }
}