using SkillQuest.API;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine;

public class Addon : Doohickey, IAddon{

    public Addon(){ }

    public override Uri Uri { get; set; } = new Uri("sh://addon.skill.quest/null");

    public virtual string Name { get; } = "null";

    public virtual string Description { get; } = "null";

    public virtual string Author { get; } = "unknown";

    public virtual string Version { get; } = "0";

    public virtual string Icon { get; } = ""; // TODO

    public virtual string Category { get; } = "";

    public IApplication? Application { 
        get {
            return _application;
        }
        set {
            if (value != _application) {
                Unmounted?.Invoke(this, _application);
                _application = value;
                Mounted?.Invoke(this, _application);
            }
        } }

    public event IAddon.DoMounted? Mounted;

    public event IAddon.DoUnmounted? Unmounted;

    IApplication? _application;
}
