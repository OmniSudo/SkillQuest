using SkillQuest.Client.Game.Addons.SkillQuest.Client;
using SkillQuest.Shared.Game;

namespace SkillQuest.Client.Desktop;

using static State;

class Program{
    static void Main(string[] args){
        SH.Application.Mount(
            new AddonSkillQuestCL()
        ).Run();
    }
}
