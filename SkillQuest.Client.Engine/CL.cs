using Silk.NET.Core.Native;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Graphics.Vulkan;

namespace SkillQuest.Client.Engine;

public class State {
    public static State CL { get; } = new State();

    public IInstance Graphics { get; set; }
}