using SkillQuest.Network;
using Steamworks;

namespace SkillQuest.Actor;

public partial class PlayerCharacter : Character {
    public Connection.Client Connection;
    
    public ulong SteamId => Connection.SteamId;
    
    public PlayerCharacter(Character.Info info) {
        this.About = info;
    }
}