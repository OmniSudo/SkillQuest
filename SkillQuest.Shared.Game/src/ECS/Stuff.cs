using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Game.ECS;

public class Stuff : IStuff {
    public event IStuff.DoThingAdded? ThingAdded;

    public event IStuff.DoThingRemoved? ThingRemoved;

    public ImmutableDictionary<Uri, IThing> Things => _things.ToImmutableDictionary();

    public IThing? Add(IThing thing){
        if (thing.Uri is null) {
            throw new NullReferenceException( nameof(thing.Uri) );
        }
        
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old == thing ) return thing;

        if (old is not null) {
            ThingRemoved?.Invoke(old);
        }
        
        _things[thing.Uri] = thing;
        ThingAdded?.Invoke(thing);

        return thing;
    }

    public IThing? Remove(IThing thing){
        var old = _things.GetValueOrDefault(thing.Uri);
        if (old != thing ) return null;

        Remove(thing.Uri!);

        return thing;
    }

    public IThing? Remove(Uri uri){
        _things.TryRemove(uri, out var thing );
        if (thing is null) return null;
        
        ThingRemoved?.Invoke(thing);
        return thing;
    }
    
    private ConcurrentDictionary<Uri, IThing> _things = new();
}