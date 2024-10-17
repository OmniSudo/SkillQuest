/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::character::select {
    class SelectCharacterPacket : public network::IPacket {
    public:
        /**
         * CLIENT->SERVER
         * Logs the client into a character
         * @param name Character to log into
         */
        explicit SelectCharacterPacket ( const std::string& name ) :
                IPacket_INIT,
                _name{ name } {
        }

        explicit SelectCharacterPacket ( const json& data ) :
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