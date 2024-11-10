using System.Collections.Concurrent;

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
    
    public ConcurrentDictionary< string, Addon > Addons { get; protected set; }
    
    public Addon? this[ string name ] { get; protected set; }
}