using Godot;
using SkillQuest.Actor;
using SkillQuest.Network;
using SkillQuest.Procedural.World;
using SkillQuest.UI.Login.Select;
using SkillQuest.World;
using Steamworks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkillQuest;

public partial class Universe : Node3D {
    public static Universe World { get; private set; }

    [Export] public WorldGenPipeline TerrainGenerator { get; private set; }

    [Export] public Node3D RegionContainer { get; set; }

    public ConcurrentDictionary<CSteamID, PlayerCharacter> Players { get; } = new();

    public ConcurrentDictionary<Vector3, Region> Regions { get; } = new();

    public override void _Ready() {
        World = this;
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
                        World.Regions[region.Position] = region;
                        Shared.SH.CallDeferred( () => {
                            World.RegionContainer.AddChild( region );
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
        if (World.Regions.ContainsKey( position )) return;

        World.Generate( position ).ContinueWith( task => {
            var region = task.Result;
            World.Regions[region.Position] = region;
            Shared.SH.CallDeferred( () => {
                World.RegionContainer.AddChild( region );
            } );
        } );
    }
}