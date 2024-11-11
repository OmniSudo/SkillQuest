using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Game.ECS;

public class Stuff : IStuff {
    public event IStuff.DoThingAdded? ThingAdded;

    public event IStuff.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IThing> Things => _things.ToImmutableDictionary();

    public IStuff Add(IThing thing){
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old == thing ) return this;

        if (old is not null) {
            ThingRemoved?.Invoke(old);
        }
        
        _things[thing.Uri] = thing;
        ThingAdded?.Invoke(thing);

        return this;
    }

    public IStuff Remove(IThing thing){
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old != thing ) return this;

        ThingRemoved?.Invoke(old);
        Remove(thing.Uri!);

        return this;
    }

    public IThing? Remove(Uri uri){
        _things.TryRemove(uri, out var thing );
        return thing;
    }
    
    private ConcurrentDictionary<Uri, IThing> _things = new();
}