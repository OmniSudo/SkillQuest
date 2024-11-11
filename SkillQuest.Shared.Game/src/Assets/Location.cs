using SkillQuest.API;

namespace SkillQuest.Shared.Game.Assets;

public class Location{
    public Uri Uri { get; }

    public Location(IAddon addon, string path){
        string side = "Shared";

        switch (addon.Uri.Scheme) {
            case "cl":
            case "client":
                side = "Client";
                break;
            case "sv":
            case "server":
                side = "Server";
                break;
        }
        Uri = new Uri( "file://" + Path.Combine($"Addons/{addon.Name}/{side}/assets/", path ) );
    }
}
