/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/block.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::block {
    class BlockInfoPacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * TODO: block properties to packet properties
		 * @param block
		 */
        explicit BlockInfoPacket ( std::shared_ptr< stuff::IThing > root ) :
                IPacket_INIT,
                _uri{ root->uri() } {
        }

        explicit BlockInfoPacket ( const json& data ) :
                network::IPacket( data ),
                _uri{ data[ "uri" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "uri" ] = uri().toString();
            return data;
        }

    property( uri, URI, public_const, public_ptr );
    };
}