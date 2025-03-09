--- @user omnisudo
--- @date 3/8/2025
--- This is an ore mined from an Iron Ore Vein.

ITEM = {}
ITEM.details = { name = {} }

ITEM.details.material = "iron"
ITEM.details.name = { ITEM.details.material .. " ore", ITEM.details.material .. " ores" }

print( ITEM.details.name[ 1 ] )