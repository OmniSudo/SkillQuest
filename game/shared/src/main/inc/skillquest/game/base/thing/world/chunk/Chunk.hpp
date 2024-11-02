/**
 * @author omnisudo
 * @date 2024.08.31
 */

#pragma once

#include "skillquest/property.hpp"
#include "skillquest/glm.hash.hpp"
#include <glm/vec3.hpp>
#include <array>
#include "skillquest/stuff.thing.hpp"
#include <memory>

namespace skillquest::game::base::thing::world {
    class World;

    namespace chunk {
        class Chunk : public stuff::Thing {
        public:
            struct CreateInfo {
                std::shared_ptr< World > world;
                glm::u8vec3 pos;
            };

            static constexpr char SIZE = 16;

            property( blocks, std::array< stuff::IThing* COMMA SIZE * SIZE * SIZE >, public_ref, public )
            property( pos, glm::u16vec3, public_const, none )

        public:
            Chunk( const CreateInfo& info );

            virtual ~Chunk() = default;

            void set( glm::u8vec3 pos, std::shared_ptr< stuff::IThing > block );

            std::shared_ptr< stuff::IThing > get ( glm::u8vec3 pos );
        };
    }
}
