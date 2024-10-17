/**
 * @author omnisudo
 * @date 2024.08.31
 */

#include "skillquest/game/base/thing/world/chunk/Chunk.hpp"
#include "skillquest/game/base/thing/world/World.hpp"

namespace skillquest::game::base::thing::world::chunk {
    Chunk::Chunk ( const Chunk::CreateInfo& info ) :
            stuff::Thing(
                    {
                            .uri = {
                                    "world://skill.quest/"
                                    + info.world->name() + "/chunk/"
                                    + std::to_string( info.pos.x ) + "."
                                    + std::to_string( info.pos.y ) + "."
                                    + std::to_string( info.pos.z )

                            }
                    }
            ),
            _pos{ info.pos } {

    }

    void Chunk::set ( glm::u8vec3 pos, std::shared_ptr< stuff::IThing > block ) {
        _blocks[ pos.x & 0xF + ( pos.y & 0xF << 4 ) + ( pos.z & 0xF << 8 ) ] = block.get();
    }

    std::shared_ptr< stuff::IThing > Chunk::get ( glm::u8vec3 pos ) {
        auto block = _blocks[ pos.x & 0xF + ( pos.y & 0xF << 4 ) + ( pos.z & 0xF << 8 ) ];
        if ( block ) return block->self();
        return nullptr;
    }
}
