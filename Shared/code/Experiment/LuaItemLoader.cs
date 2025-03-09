using Godot;
using LuaNET.Lua54;
using SkillQuest.Scripting;

namespace SkillQuest.Experiment;

[Tool]
public partial class LuaItemLoader : Node {
    public override void _Ready() {
        LuaState.DoFile( LuaState.Global, "res://addons/cooking/item/food/pizza/cheese_pizza.lua" );
    }
}