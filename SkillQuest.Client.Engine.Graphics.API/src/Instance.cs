using Silk.NET.Maths;

namespace SkillQuest.Client.Engine.Graphics.API;

public interface IInstance : IDisposable {
    public string Name { get; set; }
    public Vector2D<int> Position { get; set; }
    public Vector2D<int> Size { get; set; }

    public IntPtr WindowHandle { get; }

    public void Update(DateTime now, TimeSpan delta);
    public void Render(DateTime now, TimeSpan delta);

    public delegate void DoQuit();
    public event DoQuit? Quit;
}
