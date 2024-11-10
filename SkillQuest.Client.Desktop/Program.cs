using SkillQuest.Client.Game.Addons.Client.SkillQuest;

namespace SkillQuest.Client.Desktop;

using static SkillQuest.Shared.Game.State;

class Program{
    static void Main(string[] args){
        SH.Application.Mount(
            new AddonSkillQuestCL()
            ).Run();
    }
}