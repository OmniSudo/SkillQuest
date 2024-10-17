/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::character::select {
    class CharacterSelectInfoRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT->SERVER
         * Requests that all of the user's characters get sent to the client via a CharacterSelectInfoPacket
         */
        explicit CharacterSelectInfoRequestPacket () : IPacket_INIT {
        }

        explicit CharacterSelectInfoRequestPacket ( const json& data ) : network::IPacket( data ) {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            return data;
        }
    };
}