using Godot;
using LuaNET.Lua54;

namespace SkillQuest.Experiment;

[Tool]
public partial class LuaItemLoader : Node {
    public override void _Ready() {
        var l = Lua.luaL_newstate();

        Lua.lua_close( l );
    }
}