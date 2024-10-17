/**
 * @author omnisudo
 * @date 2024.09.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::world::chunk {
    class ChunkDataRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         */
        explicit ChunkDataRequestPacket ( const glm::u16vec3 pos ) : IPacket_INIT, _pos{ pos } {

        }

        explicit ChunkDataRequestPacket ( const json& data ) :
                network::IPacket( data ),
                _pos{ data[ "x" ].get< std::uint16_t >(), data[ "y" ].get< std::uint16_t >(), data[ "z" ].get< std::uint16_t >() }              {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "x" ] = _pos.x;
            data[ "y" ] = _pos.y;
            data[ "z" ] = _pos.z;
            return data;
        }

    property( pos, glm::u16vec3, public_const, public_ptr );
    };
}