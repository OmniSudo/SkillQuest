/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/item.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::item {
    class ItemInfoPacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * TODO: item properties to packet properties
		 * @param item
		 */
        explicit ItemInfoPacket ( std::shared_ptr< stuff::IThing > root ) :
                IPacket_INIT,
                _uri{ root->uri() } {
        }

        explicit ItemInfoPacket ( const json& data ) :
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