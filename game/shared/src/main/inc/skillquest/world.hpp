/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include "skillquest/game/base/thing/world/World.hpp"
#include "skillquest/game/base/thing/world/chunk/Chunk.hpp"

namespace sq::sh {
    typedef std::shared_ptr< skillquest::game::base::thing::world::World > World;
    typedef std::shared_ptr< skillquest::game::base::thing::world::chunk::Chunk > Chunk;
}