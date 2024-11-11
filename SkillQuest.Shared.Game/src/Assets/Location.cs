using SkillQuest.API;

namespace SkillQuest.Shared.Game.Assets;

public class Location{
    public Uri Uri { get; }
    
    public Location( IAddon addon, string path){
        Uri = new Uri( addon.Uri!, path );
    }
}
