using SkillQuest.Procedural.Node;
using SkillQuest.World;

namespace SkillQuest.Procedural.World;

public partial class EntryPointNodeWorldRegion : EntryPointNode {
    public override bool Main(Region region){
        if ( region.Universe is not null ) OnGenerate(region);

        return region.Universe is not null;
    }
}