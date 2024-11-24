using System.Collections.Concurrent;
using System.Collections.Immutable;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.ECS;

using static State;

public class Thing : IThing{

    public Thing(Uri? uri = null){
        Uri = uri;
    }

    public IStuff? Stuff {
        get {
            return _stuff;
        }
        set {
            if (value == _stuff)
                return;

            if (_stuff is not null) {
                Unstuffed?.Invoke(_stuff, this);
                _stuff.Remove(this);
            }
            _stuff = value ?? SH.Stuff;
            _stuff.Add(this);
            Stuffed?.Invoke(_stuff, this);
        }
    }


    public virtual Uri? Uri { get; }

    public event IThing.DoStuffed Stuffed;

    public event IThing.DoUnstuffed Unstuffed;

    public event IThing.DoConnectComponent ConnectComponent;

    public event IThing.DoDisconnectComponent DisconnectComponent;

    public event IThing.DoParented Parented;

    public event IThing.DoUnparented Unparented;

    public event IThing.DoAddChild AddChild;

    public event IThing.DoRemoveChild RemoveChild;

    public IThing Component<TComponent>(TComponent? component) where TComponent : class, IComponent{
        return Component(component, typeof(TComponent));
    }

    public IThing Component(IComponent? component, Type? type = null){
        type ??= component?.GetType();

        if (type is not null) {
            if (component is not null) {
                var old = Components.GetValueOrDefault(type);

                if (old == component) {
                    return this;
                }

                component.Thing = this;
                _components[type] = component;

                ConnectComponent?.Invoke(this, component);
            } else {
                _components.TryRemove( type, out var removed );
                if (removed is null) return this;
                DisconnectComponent?.Invoke(this, removed);
            }
        }
        return this;
    }

    public TComponent? Component<TComponent>(object component) where TComponent : class, IComponent =>
        Component(typeof(TComponent)) as TComponent;


    public IComponent? Component(Type type) => Components.GetValueOrDefault(type);

    public IComponent? this[Type type] {
        get => Component(type);
        set => Component(value, type);
    }

    public ImmutableDictionary<Type, IComponent> Components => _components.ToImmutableDictionary();

    ConcurrentDictionary<Type, IComponent> _components = new();

    public IThing? Parent {
        get {
            return _parent;
        }
        set {
            if (value == _parent) return;

            var old = _parent;

            _parent = value;

            if (old is not null) {
                old[this.Uri] = null;
            }

            if (_parent is not null) {
                _parent[this.Uri] = this;
            }

            if (value is null) {
                Unparented?.Invoke(old, this);
            } else {
                Parented?.Invoke(value, this);
            }
        }
    }

    public ImmutableDictionary<Uri, IThing> Children => _children.ToImmutableDictionary();

    private ConcurrentDictionary<Uri, IThing> _children = new();

    public IThing this[Uri uri] {
        get {
            return Children.GetValueOrDefault(uri);
        }
        set {
            IThing old = _children.GetValueOrDefault(uri);
            if (old == value) return;

            if (value is null && old is not null) {
                _children.TryRemove(uri, out var _);
                old.Parent = null;
                RemoveChild?.Invoke(this, old);
                return;
            }


            if (value is not null) {
                _children[uri] = value;
                value.Parent = this;
                AddChild?.Invoke(this, value);
            }
        }
    }

    public bool this[IThing thing] {
        get {
            return this[thing.Uri] == thing;
        }
        set {
            if (value) {
                this[thing.Uri] = thing;
            } else {
                if (_children.TryGetValue(thing.Uri, out var current) && current == thing) {
                    this[thing.Uri] = null;
                }
            }
        }
    }

    IStuff? _stuff;

    IThing? _parent = null;
}