/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::packet::character::select {
    class CharacterSelectInfoPacket : public network::IPacket {
    public:
        /**
         * SERVER->CLIENT
         * TODO: Character properties to packet data
         * @param uri Character URI
         * @param name Character name, displayed ingame
         */
        explicit CharacterSelectInfoPacket ( const URI& uri, const std::string& name, const util::UID& uid ) :
                IPacket_INIT,
                _uri{ uri },
                _uid{ uid },
                _name{ name } {

        }

        explicit CharacterSelectInfoPacket ( const json& data ) :
                network::IPacket( data ),
                _uri{ data[ "uri" ].get< std::string >() },
                _uid{ data[ "uid" ].get< std::string >() },
                _name{ data[ "name" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
                data[ "uri" ] = uri().toString();
                data[ "uid" ] = uid().toString();
                data[ "name" ] = name();
            return data;
        }

    property( uri, URI, public_const, public_ptr );
    property( uid, util::UID, public_const, public_ptr );
    property( name, std::string, public_const, public_ptr );
    };
}