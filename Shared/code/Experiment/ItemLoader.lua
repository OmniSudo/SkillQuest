--- @user omnisudo
--- @date 3/8/2025


-- TODO: Modularize config types to use metatable __newindex function
__metatable = {
    __index = function ( t, k )
        return rawget( t, k )
    end,
    __newindex = function ( t, k, v )
        if ( k == "RECIPE" ) then
            
        else
            rawset( t, k, v )
        end
    end,
    
}

setmetatable( _G, __metatable)