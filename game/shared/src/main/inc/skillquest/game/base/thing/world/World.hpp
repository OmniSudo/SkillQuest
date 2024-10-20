/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/character.hpp"
#include "skillquest/glm.hash.hpp"
#include <glm/vec3.hpp>
#include "chunk/Chunk.hpp"

namespace skillquest::game::base::thing::world {
    class World : public stuff::Doohickey {
    public:
        struct CreateInfo {
            network::Channel*& channel;
            std::string name;
        };

    public:
        inline static URI world_uri ( std::string name ) {
            return { "world://skill.quest/world/" + name };
        }

        explicit World ( const CreateInfo& info );

        ~World () override;

        virtual std::shared_ptr< character::PlayerCharacter > add_player (
                std::shared_ptr< character::PlayerCharacter > player
        );

    property(
            players_by_name,
            std::map< std::string COMMA std::shared_ptr< character::PlayerCharacter > >,
            public_ref,            none
    );

    property( channel, network::Channel*&, public_ref, none );

    property( name, std::string, public, none );

    property( seed, std::size_t, public, none );

        virtual bool remove_player ( std::shared_ptr< character::PlayerCharacter > player );

        virtual bool remove_player ( const util::UID& user_uid );

    property( chunks, std::unordered_map< glm::u16vec3 COMMA std::shared_ptr< world::chunk::Chunk > >, public_ref, none );

        std::shared_ptr< world::chunk::Chunk > chunk ( glm::u16vec3 pos );

        std::shared_ptr< stuff::IThing > block ( glm::u64vec3 pos );

        void block ( glm::u64vec3 pos, std::shared_ptr< stuff::IThing > block );
        
    };
}