using Godot;
using SkillQuest.Actor;
using SkillQuest.Network;
using SkillQuest.Procedural.World;
using SkillQuest.UI.Login.Select;
using SkillQuest.Terrain;
using Steamworks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillQuest;

public partial class World : Node3D {
    public static World Overworld { get; private set; }

    [Export] public WorldGenPipeline TerrainGenerator { get; private set; }

    [Export] public Node3D RegionContainer { get; set; }

    public ConcurrentDictionary<ulong, PlayerCharacter> Players { get; } = new();

    public ConcurrentDictionary<Vector3, Region> Regions { get; } = new();

    public override void _Ready() {
        Overworld = this;
        PlayersStorage = new Node() { Name = "Players" };
        AddChild(PlayersStorage);
    }

    public async Task<Region?> Generate(Vector3 position) {
        return await Generate( position, TerrainGenerator );
    }

    public async Task<Region?> Generate(Vector3 position, WorldGenPipeline pipeline) {
        return await pipeline.Generate( this, position );
    }

    public void Generate(Connection.Client client, Vector3 position) {
        if (!Regions.ContainsKey( position )) {
            var task = Generate( position );
            task.ContinueWith( t => {
                    var region = t.Result;

                    if (region is not null) {
                        Overworld.Regions[region.Position] = region;
                        Shared.SH.CallDeferred( () => {
                            Overworld.RegionContainer.AddChild( region );
                        } );
                    }
                }
            );
        }

        if (!Server.IsDedicated) return;

        using (Network.Rpc.FilterInclude( client )) {
            _CL_Generate( position.X, position.Y, position.Z );
        }
    }

    [Broadcast]
    private static async void _CL_Generate(float x, float y, float z) {
        var position = new Vector3( x, y, z );
        if (Overworld.Regions.ContainsKey( position )) return;

        Overworld.Generate( position ).ContinueWith( task => {
            var region = task.Result;
            Overworld.Regions[region.Position] = region;
            Shared.SH.CallDeferred( () => {
                Overworld.RegionContainer.AddChild( region );
            } );
        } );
    }

    private Node PlayersStorage;

    public void AddPlayer(PlayerCharacter player) {
        Players[ player.SteamId ] = player;
        
        player.SetName( player.About.Name );
        PlayersStorage.AddChild( player );
    }

    public void RemovePlayer(PlayerCharacter player) {
        Players.TryRemove( player.SteamId, out _);
    }
}