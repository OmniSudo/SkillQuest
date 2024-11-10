namespace SkillQuest.Shared.Game;

public abstract class Addon{
    public virtual string Name { get; protected set; } = "Addon";

    public virtual string Description { get; protected set; } = "Default Addon";

    public virtual string Author { get; protected set; } = "SkillQuest";

    public virtual string Version { get; protected set; } = "0";

    public virtual string Icon { get; protected set; } = "";

    public virtual string Category { get; protected set; } = "";

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
        }
    }

    public delegate void DoMounted(Addon addon, IApplication? application);

    public event DoMounted Mounted;

    public delegate void DoUnmounted(Addon addon, IApplication? application);

    public event DoUnmounted Unmounted;

    private IApplication? _application = null;
}