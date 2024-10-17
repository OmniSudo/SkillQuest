/**
 * @author omnisudo
 * @date 2024.08.05
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::packet::character::create {
    class CreateCharacterRequestPacket : public network::IPacket {
    public:
        /**
         * SERVER->CLIENT
         * TODO: Character properties to packet data
         * @param success Was the character successfully created
         * @param reason Reason for this packet
         */
        explicit CreateCharacterRequestPacket ( const std::string& name ) :
                IPacket_INIT,
                _name{name} {

        }

        explicit CreateCharacterRequestPacket ( const json& data ) :
                network::IPacket( data ),
                _name{ data[ "name" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "name" ] = name();
            return data;
        }

    property( name, std::string, public_const, public_ptr );
    };
}