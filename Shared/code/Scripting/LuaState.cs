using Godot;
using LuaNET.Lua54;

namespace SkillQuest.Scripting;

public class LuaState {
    public static lua_State Global = Lua.luaL_newstate();
}