/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"
#include "skillquest/game/base/thing/world/World.hpp"

namespace skillquest::game::base::packet::character {
    class CharacterJoinWorldPacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * Tells the client to add a character
         * @param name Character to log into
         */
        explicit CharacterJoinWorldPacket ( std::shared_ptr< thing::world::World > world, std::shared_ptr< thing::character::PlayerCharacter > player ) :
                IPacket_INIT,
                _name{ player->name() },
                _uid{ player->uid() },
                _world{ world->name() } {
        }

        explicit CharacterJoinWorldPacket ( const json& data ) :
                network::IPacket( data ),
                _name{ data[ "name" ].get< std::string >() },
                _uid{ data[ "uid" ].get< std::string >() },
                _world{ data[ "world" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "name" ] = name();
            data[ "uid" ] = uid().toString();
            data[ "world" ] = world();
            return data;
        }

        property( name, std::string, public_const, public_ptr );
        property( uid, util::UID, public_const, public_ptr );
        property( world, std::string, public_const, public_ptr );
    };
}