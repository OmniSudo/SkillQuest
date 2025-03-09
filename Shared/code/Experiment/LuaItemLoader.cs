using Godot;
using LuaNET.Lua54;
using SkillQuest.Scripting;

namespace SkillQuest.Experiment;

[Tool]
public partial class LuaItemLoader : Node {
    public override void _Ready() {
        LuaState.Global.DoFile( "res://Shared/assets/items/mining/ore/IronOre.lua");
    }
}