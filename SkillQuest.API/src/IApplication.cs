using System.Collections.Immutable;

namespace SkillQuest.API;

public interface IApplication{
    public bool Running { get; set; }

    public uint TicksPerSecond();

    public TimeSpan TickFrequency();

    public IApplication Mount(IAddon addon);

    public IApplication Unmount(IAddon? addon);

    public void Run();

    public delegate bool DoStart();

    public event DoStart Start;

    public delegate void DoUpdate();

    public event DoUpdate Update;

    public delegate bool DoStop();

    public event DoStop Stop;
    
    public ImmutableDictionary<Uri, IAddon> Addons { get; }
    
    public IAddon? this[ Uri uri ] { get; }
}