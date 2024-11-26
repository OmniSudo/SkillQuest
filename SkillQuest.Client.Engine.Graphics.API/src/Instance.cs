using Silk.NET.Maths;

namespace SkillQuest.Client.Engine.Graphics.API;

public interface IInstance : IDisposable {
    public string Name { get; set; }
    public Vector2D<int> Position { get; set; }
    public Vector2D<int> Size { get; set; }
}
