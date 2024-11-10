using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SkillQuest.Shared.Game;

public interface IApplication{
    public bool Running { get; set; }

    public uint TicksPerSecond();

    public TimeSpan TickFrequency();

    public IApplication Mount(Addon addon);

    public IApplication Unmount(Addon addon);

    public void Run();

    public delegate bool DoStart();

    public event DoStart Start;

    public delegate void DoUpdate();

    public event DoUpdate Update;

    public delegate bool DoStop();

    public event DoStop Stop;
    
    public ImmutableDictionary<string, Addon> Addons { get; }
    
    public Addon? this[ string name ] { get; protected set; }
}