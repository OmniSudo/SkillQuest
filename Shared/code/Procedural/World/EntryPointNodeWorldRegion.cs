using SkillQuest.Procedural.Node;
using SkillQuest.Terrain;

namespace SkillQuest.Procedural.World;

public partial class EntryPointNodeWorldRegion : EntryPointNode {
    public override bool Main(Region region){
        if ( region.World is not null ) OnGenerate(region);

        return region.World is not null;
    }
}