using SkillQuest.Procedural.Node;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SkillQuest.Procedural;

public partial class ProcGenPipeline : Godot.Node {
    public async Task<ImmutableList<EntryPointNode>> EntryPoints() {
        var tcs = new TaskCompletionSource<ImmutableList<EntryPointNode>>();

        Shared.SH.CallDeferred( () => {
            if (tcs.Task.IsCompleted) return;
            tcs.SetResult(
                GetChildren()
                    .Where( node => node is EntryPointNode )
                    .Cast<EntryPointNode>()
                    .ToImmutableList()
            );
        } );

        return await tcs.Task;
    }
}