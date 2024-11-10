using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace SkillQuest.Shared.Game;

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

    public Application(){
    }

    uint IApplication.TicksPerSecond() => 100;

    TimeSpan IApplication.TickFrequency() => TimeSpan.FromSeconds(1) / TicksPerSecond;

    public IApplication Mount(Addon addon){
        _addons[addon.Name] = addon;
        addon.Application = this;
        return this;
    }

    public IApplication Unmount(Addon addon) {
        if (addon is null) {
            var copy = new Dictionary<string, Addon>( Addons );
            foreach (var pair in copy) {
                pair.Value.Application = null;
                _addons.TryRemove(pair.Key, out _);
            }
        } else if (Addons.ContainsKey( addon.Name ) && Addons[addon.Name] == addon) {
            _addons.TryRemove(addon.Name, out var rm );
            addon.Application = null;
        }
        return this;
    }

    public void Run(){
        Start?.Invoke();

        var previous = DateTime.Now;
        var total = TimeSpan.Zero;

        while ( Running ) {
            var delta = previous - DateTime.Now;
            total += delta;

            while ( total > TickFrequency ) {
                if (total > TimeSpan.FromSeconds(1)) {
                    total = TimeSpan.FromSeconds(1);
                }

                Update?.Invoke();
                
                total -= TickFrequency;
            }
        }

        Stop?.Invoke();
    }

    public event IApplication.DoStart? Start;

    public event IApplication.DoUpdate? Update;

    public event IApplication.DoStop? Stop;

    public ImmutableDictionary<string, Addon > Addons => _addons.ToImmutableDictionary();

    private ConcurrentDictionary<string, Addon> _addons = new();
    
    public Addon? this[string name] {
        get {
            return Addons.GetValueOrDefault(name);
        }
        set {
            if (value == null) {
                _addons.TryRemove(name, out _);
            }
            else {
                var old = Addons.GetValueOrDefault(name);

                if (old is not null) {
                    old.Application = null;
                }
                _addons[name] = value;
                value.Application = this;
            }
        }
    }

    bool _running = false;
}