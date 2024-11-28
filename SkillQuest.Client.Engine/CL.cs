using Silk.NET.Core.Native;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Client.Engine;

public class State {
    public static State CL { get; } = new State();

    public IGraphicsInstance Graphics { get; set; }
}