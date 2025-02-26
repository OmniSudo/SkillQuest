using Godot;
using MethodDecorator.Fody.Interfaces;
using SkillQuest.Core;
using SkillQuest.Packet.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SkillQuest.Network;

public static class Rpc {
    private static Connection.Client _sender;

    public static Connection.Client Caller { get; internal set; }

    public static bool Calling { get; internal set; }

    public static Connection.Filter? Filter { get; set; }

    public static Task Wait => _tcs.Task;

    private static TaskCompletionSource _tcs = new();

    public static IDisposable FilterInclude(IEnumerable<Connection.Client> connections) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );

        Filter = new(Connection.Filter.FilterType.Include, connections);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }

    /// <summary>
    /// Filter the recipients of any Rpc called in this scope to only include a <see cref="Connection"/> based on a predicate.
    /// </summary>
    /// <param name="predicate">Only send the RPC to connections that meet the criteria of the predicate.</param>
    public static IDisposable FilterInclude(Predicate<Connection.Client> predicate) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );

        Filter = new(Connection.Filter.FilterType.Include, predicate);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }

    /// <summary>
    /// Filter the recipients of any Rpc called in this scope to only include the specified <see cref="Connection"/>.
    /// </summary>
    /// <param name="connection">Only send the RPC to this connection.</param>
    public static IDisposable FilterInclude(Connection.Client connection) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );

        Filter = new(Connection.Filter.FilterType.Include, c => c == connection);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }

    /// <summary>
    /// Filter the recipients of any Rpc called in this scope to exclude a <see cref="Connection"/> based on a predicate.
    /// </summary>
    /// <param name="predicate">Exclude connections that don't meet the criteria of the predicate from receiving the RPC.</param>
    public static IDisposable FilterExclude(Predicate<Connection.Client> predicate) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );
        Filter = new(Connection.Filter.FilterType.Exclude, predicate);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }

    /// <summary>
    /// Filter the recipients of any Rpc called in this scope to exclude the specified <see cref="Connection"/> set.
    /// </summary>
    /// <param name="connections">Exclude these connections from receiving the RPC.</param>
    public static IDisposable FilterExclude(IEnumerable<Connection.Client> connections) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );

        Filter = new(Connection.Filter.FilterType.Exclude, connections);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }

    /// <summary>
    /// Filter the recipients of any Rpc called in this scope to exclude the specified <see cref="Connection"/>.
    /// </summary>
    /// <param name="connection">Exclude this connection from receiving the RPC.</param>
    public static IDisposable FilterExclude(Connection.Client connection) {
        if (Filter.HasValue)
            throw new InvalidOperationException( "An RPC filter is already active" );

        Filter = new(Connection.Filter.FilterType.Exclude, c => c == connection);

        return DisposeAction.Create( () => {
            Filter = null;
        } );
    }
}

public abstract class RpcAttribute : Attribute, IMethodDecorator {
    public void Init(object instance, MethodBase method, object[] args) {
    }

    public void OnEntry() {

    }

    public void OnExit() {
        Rpc.Filter = _filter;
    }

    public void OnException(Exception exception) {
        Rpc.Filter = _filter;
        GD.PrintErr( "OnException: " + exception.Message );
    }


    protected Connection.Filter? _filter;

    static RpcAttribute() {
        _channel = Shared.Multiplayer.CreateChannel( new Uri( "packet://rpc.skill.quest/" ) );
        _channel.Subscribe<RpcPacket>( OnRpcPacket );
    }

    private static void OnRpcPacket(Connection.Client connection, RpcPacket packet) {
        var type = Type.GetType( packet.TypeName );
        var method = type.GetMethod( packet.MethodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static );
        if (method?.GetCustomAttribute<HostAttribute>() is null &&
            method?.GetCustomAttribute<BroadcastAttribute>() is null) {
            return;
        }

        Rpc.Caller = connection;
        try {
            var args = new List<object>();
            foreach (var param in method.GetParameters()) {
                args.Add( JsonSerializer.Deserialize( packet.Arguments[args.Count], param.ParameterType ) );
            }

            method?.Invoke( null, args.ToArray() );
        } catch (Exception e) {
            GD.PrintErr( $"Failed to finish invoking RPC request {e.Message}: {e.StackTrace}" );
        }

        Rpc.Caller = null;
    }

    protected static Channel _channel;
}

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class HostAttribute : RpcAttribute {
    public bool NeedBypass() {
        return Multiplayer.Host is not null || (!Rpc.Filter?.IsRecipient( Multiplayer.Host ) ?? false);
    }

    public void Init(object instance, MethodBase method, object[] args) {
        if (Multiplayer.Host is not null) {
            var _packet = new RpcPacket() {
                MethodName = method.Name,
                TypeName = method.DeclaringType.AssemblyQualifiedName,
                Arguments = JsonSerializer.SerializeToNode( args ).AsArray(),
            };

            _channel.Send( Multiplayer.Host, _packet, true );
        }
    }

    public void OnEntry() {
        if (!Rpc.Filter?.IsRecipient( Multiplayer.Host ) ?? false) throw new OperationCanceledException();
        Rpc.Caller = Multiplayer.Host;

        _filter = Rpc.Filter;
    }
}

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class BroadcastAttribute : RpcAttribute {
    public bool NeedBypass() {
        return Multiplayer.Host is not null && (!Rpc.Filter?.IsRecipient( Multiplayer.Host ) ?? false);
    }

    public void Init(object instance, MethodBase method, object[] args) {
        if (Multiplayer.Host is null) {
            var _packet = new RpcPacket() {
                MethodName = method.Name,
                TypeName = method.DeclaringType.AssemblyQualifiedName,
                Arguments = JsonSerializer.SerializeToNode( args ).AsArray(),
            };

            foreach (
                var connection in
                Shared.Multiplayer.Clients.Values.Where( (c) => (Rpc.Filter?.IsRecipient( c ) ?? true))) {
                _channel.Send( connection, _packet, true );
            }
        }
    }

    public void OnEntry() {
        Rpc.Caller = Multiplayer.Host;

        _filter = Rpc.Filter;
    }
}