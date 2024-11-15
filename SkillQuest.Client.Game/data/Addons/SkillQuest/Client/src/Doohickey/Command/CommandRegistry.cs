using System.Collections.Concurrent;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Command;

public class CommandRegistry : Shared.Game.ECS.Doohickey{
    public override Uri? Uri { get; } = new Uri("cl://skill.quest/commands");

    public async Task Post(string text){
        var cmd = text.Split(' ').First();

        if (_commands.ContainsKey(cmd)) {
            _commands[cmd]?.Invoke(cmd, text);
        }
    }
    
    public void Subscribe(string cmd, DoCommand doCommand){
        if (!_commands.ContainsKey(cmd)) {
            _commands.TryAdd(cmd, doCommand);
        } else {
            _commands[ cmd ] += doCommand;
        }
    }

    public void Unsubscribe(string cmd, DoCommand doCommand){
        if (_commands.ContainsKey(cmd)) {
            _commands[ cmd ] -= doCommand;
        }
    }

    public async Task Listen( bool hidden = false ){
        ConsoleKey key;
        string cmd = "";
        
        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && cmd.Length > 0) {
                Console.Write("\b \b");
                cmd = cmd[0..^1];
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                Console.Write(hidden ? "*" : keyInfo.KeyChar);
                cmd += keyInfo.KeyChar;
            }
        } while ( key != ConsoleKey.Enter );

        await Post(cmd);
    }

    public delegate void DoCommand(string cmd, string line);

    ConcurrentDictionary<string, DoCommand> _commands = new();
}
