using Silk.NET.Maths;
using SkillQuest.API.Thing.Universe;

namespace SkillQuest.API.Procedural.World;

public interface IWorldGenPipeline : IProcGenPipeline{
    public IRegion Generate(IWorld world, Vector3D<long> position);
}
