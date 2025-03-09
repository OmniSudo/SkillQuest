using Godot;
using LuaNET.Lua54;

namespace SkillQuest.Scripting;

public static class LuaState {
    public static lua_State Global = Lua.luaL_newstate();

    static LuaState() {
        Lua.luaopen_base( Global );
        Lua.luaopen_math( Global );
        Lua.luaopen_table( Global );
        Lua.luaopen_string( Global );
        Lua.luaopen_coroutine( Global );

        Lua.lua_register( Global, "print", Print );
        Lua.lua_register( Global, "require", Require );
    }

    private static int Print(lua_State state) {
        var L = state;
        int nargs = Lua.lua_gettop( L );

        string s = "";

        for (int i = 1; i <= nargs; i++) {
            if (s.Length != 0) s += "\t";

            int t = Lua.lua_type( L, i );
            switch (t) {
                case Lua.LUA_TSTRING:
                {
                    /* strings */
                    s += Lua.lua_tostring( L, i );
                    break;
                }
                case Lua.LUA_TBOOLEAN:
                {
                    /* booleans */
                    s += (Lua.lua_toboolean( L, i ) != 0 ? "true" : "false");
                    break;
                }
                case Lua.LUA_TNUMBER:
                {
                    /* numbers */
                    s += Lua.lua_tonumber( L, i );
                    break;
                }
                case Lua.LUA_TNIL:
                {
                    s += "nil";
                    break;
                }
                default:
                {
                    /* other values */
                    s += Lua.lua_typename( L, t ) + ": " + Lua.lua_topointer( L, i );
                    break;
                }
            }
        }

        GD.Print( s );
        return 0;
    }

    private static int Require(lua_State l) {
        if (Lua.lua_isstring( l, 1 ) == 0) return 0;
        var path = Lua.lua_tostring( l, 1 );

        var res = $"res://addons/{path.Replace( '.', '/' )}.lua";
        if (FileAccess.FileExists( res )) {
            Lua.luaL_dofile( l, ProjectSettings.GlobalizePath( res ) );
            return 1;
        }

        return 0;
    }

    public static bool Check(this lua_State state, int ret) {
        if (ret != Lua.LUA_OK) {
            var error = Lua.lua_tostring( state, -1 );
            GD.PrintErr( error );
            return false;
        }

        return true;
    }

    public static bool DoFile(this lua_State state, string respath) {
        return Global.Check( Lua.luaL_dofile( state, ProjectSettings.GlobalizePath( respath ) ) );
    }
}