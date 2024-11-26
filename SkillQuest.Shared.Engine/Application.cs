using System.Collections.Immutable;
using SkillQuest.API;

namespace SkillQuest.Shared.Engine;

using static State;

public class Application : IApplication{
    public bool Running {
        get {
            return _running;
        }
        set {
            if (_running == value)
                return;

            if (value) {
                _running = true;
                Run();
                return;
            }
            _running = false;
        }
    }

    public uint TicksPerSecond => 100;

    public TimeSpan TickFrequency {
        get {
            return TimeSpan.FromSeconds(1) / TicksPerSecond;
        }
    }

    public Application(){ }

    public IApplication Mount(IAddon addon){
        _addons[ addon.Uri ] = SH.Stuff.Add(addon);
        addon.Application = this;
        return this;
    }
    
    private Dictionary< Uri, IAddon > _addons = new();

    public IApplication Unmount(IAddon? addon){
        if (addon is null) {
            foreach ( var pair in Addons) {
                pair.Value.Application = null;
                SH.Stuff.Remove(pair.Value);
            }
        } else if (SH.Stuff.Things.ContainsKey(addon.Uri!)) {
            SH.Stuff.Remove(addon);
            addon.Application = null;
        }
        return this;
    }

    public void Run(){
        Start?.Invoke();

        _running = true;

        Loop();

        Stop?.Invoke();
    }

    public virtual void Loop(){
        while ( Running ){
            OnUpdate();
        }
    }

    public event IApplication.DoStart? Start;

    public event IApplication.DoUpdate? Update;

    protected void OnUpdate(){
        Update?.Invoke();
    }
    
    public event IApplication.DoRender? Render;

    protected void OnRender(){
        Render?.Invoke();
    }
    
    public event IApplication.DoStop? Stop;

    public ImmutableDictionary<Uri, IAddon> Addons => SH.Stuff.Things
        .Where(
            (pair) => pair.Value is IAddon
            )
        .ToImmutableDictionary(
            pair => pair.Key, pair => pair.Value as IAddon
        )!;

    public IAddon? this[Uri uri] {
        get {
            return Addons.GetValueOrDefault(uri) as IAddon;
        }
        set {
            if (value == null) {
                SH.Stuff.Remove(uri);
            } else {
                var old = Addons.GetValueOrDefault(uri);

                if (old is not null) {
                    SH.Stuff.Remove(old);
                    old.Application = null;
                }
                SH.Stuff.Add( value );
                value.Application = this;
            }
        }
    }

    bool _running = false;
}
