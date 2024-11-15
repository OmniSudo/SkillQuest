using SkillQuest.API.ECS;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Command;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui;

public class GuiMine : Shared.Game.ECS.Doohickey, IRenderable {
    public GuiMine (){
        AddonSkillQuestCL.Commands.Subscribe( "mine", DoMineCommand);
    }

    void DoMineCommand(string cmd, string line){
        Console.WriteLine( $"CMD {cmd}: {line}" );
    }

    public async Task Render(){
        
    }
}
