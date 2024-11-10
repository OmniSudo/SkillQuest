using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkillQuest.Shared.Game.ECS;

public interface IThing{
    public Uri Uri { get; protected set; }
    
    public delegate void DoConnectComponent ( IThing thing, IComponent component );
    
    public event DoConnectComponent ConnectComponent;

    public delegate void DoDisconnectComponent ( IThing thing, IComponent component );
    
    public event DoDisconnectComponent DisconnectComponent;

    public delegate void DoParented(IThing parent, IThing child);
    
    public event DoParented Parented;

    public delegate void DoUnparented(IThing parent, IThing child);
    
    public event DoUnparented Unparented;

    public delegate void DoAddChild(IThing parent, IThing child);
    
    public event DoAddChild AddChild;

    public delegate void DoRemoveChild(IThing parent, IThing child);
    
    public event DoRemoveChild RemoveChild;

    IThing Component<TComponent>(TComponent? component) where TComponent : class, IComponent;

    IThing Component( IComponent? component, Type? type = null );

    void Component<TAttached>(object component) where TAttached : class, IComponent{
        Component(typeof( TAttached ) );
    }

    IComponent? Component(Type type);

    public IComponent? this[Type type] {
        get;
        set;
    }

    ImmutableDictionary<Type, IComponent> Components { get; }

    public IThing? Parent { get; set; }

    public ImmutableDictionary<Uri, IThing> Children { get; }
    
    public IThing? this[Uri uri] { get; set; }
    
    public bool this[ IThing thing ] { get; set; }
}
