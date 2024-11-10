namespace SkillQuest.Shared.Game.ECS;

public class Component<TAttached> : IComponent where TAttached : class, IComponent{
    public IThing? Thing {
        get {
            return _thing;
        }
        set {
            if ( _thing != value ) {
                var old = _thing;
                
                if ( old is not null ) {
                    DisconnectThing?.Invoke(old, this);
                    old.Component( null, this.GetType() );
                }
                
                _thing = value;
                
                if (_thing is not null) {
                    _thing.Component<TAttached>(this);
                    ConnectThing?.Invoke(_thing, this);
                }
            }
        }
    }

    public event IComponent.DoConnectThing? ConnectThing;

    public event IComponent.DoDisconnectThing? DisconnectThing;

    public string Name { get; set; }
    
    IThing? _thing;
}
