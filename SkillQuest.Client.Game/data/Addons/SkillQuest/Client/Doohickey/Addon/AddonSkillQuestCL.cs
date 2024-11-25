using SkillQuest.API;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Addon;

public class AddonSkillQuestCL : AddonSkillQuestSH {
    public override Uri Uri { get; } = new Uri("cl://addon.skill.quest/skillquest");

    public override string Description { get; } = "Base Game";
    
    public AddonSkillQuestCL(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void OnMounted(IAddon addon, IApplication? application){
        Stuff.ThingAdded += StuffOnThingAdded;
        Stuff.ThingRemoved += StuffOnThingRemoved;
        
        application.Render += OnRender;

        Stuff.Add(new GuiMainMenu());
    }

    Dictionary<Uri, IRenderable> _renderables = new();
    
    void StuffOnThingRemoved(IThing thing){
        if (thing is IRenderable) {
            _renderables.Remove( thing.Uri );
        }
    }

    void StuffOnThingAdded(IThing thing){
        if (thing is IRenderable renderable) {
            _renderables[ thing.Uri ] = renderable;
        }
    }

    void OnRender(){
        foreach (var pair in _renderables) {
            pair.Value.Render();
        }
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }
}
