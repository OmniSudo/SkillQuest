﻿using Godot;
using SkillQuest.Procedural.Node;
using SkillQuest.World;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SkillQuest.Procedural.World;

public partial class WorldGenPipeline : ProcGenPipeline {
    public async Task<Region?> Generate(SkillQuest.Universe universe, Vector3 position) {
        var region = new Region(universe, position);

        List<Task> tasks;

        var ep = await EntryPoints();
        if (ep is null) return null;
        
        if (ep.Count == 0) {
            tasks = new List<Task>();
        } else {
            tasks = new List<Task>(ep.Count);
        }

        foreach (var main in ep) {
            if (main is EntryPointNodeWorldRegion node) {
                tasks.Add(
                    Task.Run(() => {
                        node.Main(region); // TODO: Use return value of Main
                    })
                );
            }
        }

        foreach (var task in tasks) {
            task.Wait();
        }

        return region;
    }
}