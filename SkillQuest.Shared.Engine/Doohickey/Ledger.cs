using System.Xml.Linq;
using SkillQuest.API.ECS;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Thing;

namespace SkillQuest.Shared.Engine.Doohickey.Ledger;

using static Engine.State;

public class Ledger < TTracked > : IDisposable where TTracked : class, IThing, new(){
    Stuff _stuff = new();
    
    public Ledger(){
        _stuff.ThingAdded += StuffOnThingAdded;
        _stuff.ThingRemoved += StuffOnThingRemoved;
    }

    void StuffOnThingAdded(IThing thing){
        if (thing is TTracked tracked) {
            Added?.Invoke(this, tracked);
        }
    }

    void StuffOnThingRemoved(IThing thing){
        if (thing is TTracked tracked) {
            Removed?.Invoke(this, tracked);
        }
    }

    public TTracked? this[Uri uri] {
        get {
            if (_stuff.Things.TryGetValue(uri, out var thing)) {
                return thing as TTracked;
            } else {
                var item = new TTracked() {
                    Uri = uri,
                };
                _stuff.Add(item);
                return item;
            }
        }
        set {
            if (value is null) {
                _stuff.Remove(uri);
            } else {
                var old = _stuff.Things[uri] as Thing.Item;

                if (old != value) {
                    value.Uri = uri;
                    var item = _stuff.Add(value) as Thing.Item;
                }
            }
        }
    }
    
    public delegate void DoAdded(Ledger<TTracked> ledger, TTracked item);
    
    public event DoAdded Added;
    
    public delegate void DoRemoved(Ledger<TTracked> ledger, TTracked item);
    
    public event DoRemoved Removed;

    public void Dispose(){
        foreach (var item in _stuff.Things.Where( thing => thing is IItem )) {
            item.Value.Dispose();
        }
    }
}
