using SkillQuest.Client.Game;
using SkillQuest.Client.Game.Addons.SkillQuest.Client;
using State = SkillQuest.Shared.Game.State;

namespace SkillQuest.Client.Desktop;

using static State;

class Program{
    static void Main(string[] args){
        ( SH.Application = new ClientApplication() ).Run();

        SH.Application.Mount(
            new AddonSkillQuestCL()
        );
    }
}
