/**
 * @author omnisudo
 * @date 2024.08.05
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::packet::character::create {
    class CharacterCreatedResponsePacket : public network::IPacket {
    public:
        /**
         * SERVER->CLIENT
         * TODO: Character properties to packet data
         * @param success Was the character successfully created
         * @param reason Reason for this packet
         */
        explicit CharacterCreatedResponsePacket ( bool success, const std::string& reason = "" ) :
                IPacket_INIT,
                _success{ success },
                _reason{ reason } {

        }

        explicit CharacterCreatedResponsePacket ( const json& data ) :
                network::IPacket( data ),
                _success{ data[ "success" ].get< bool >() },
                _reason{ data[ "reason" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "success" ] = success();
            data[ "reason" ] = reason();
            return data;
        }

    property( success, bool, public_const, public_ptr );
    property( reason, std::string, public_const, public_ptr );
    };
}