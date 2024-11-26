using Silk.NET.Maths;
using SkillQuest.API.ECS;
using SkillQuest.API.Procedural.World;
using SkillQuest.API.Thing.Universe;
using SkillQuest.Shared.Engine.ECS;
using SkillQuest.Shared.Engine.Thing.Universe;

namespace SkillQuest.Shared.Engine.Procedural.World;

public class WorldGenerationPipeline : IWorldGenPipeline{
    public IStuff Stuff {
        get;
    } = new Stuff();

    public IRegion Generate(IWorld world, Vector3D<long> position){
        var region = new Region( world, position );
        throw new NotImplementedException();
        /**
         * TODO: Find entry points in stuff and evaluate, following trail of URIs to doohickeys that output region data
         */
    }
}
