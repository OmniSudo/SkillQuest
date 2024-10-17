/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::item {
	class ItemInfoRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         */
        explicit ItemInfoRequestPacket ( const URI& uri ) : IPacket_INIT, _uri{ uri } {

        }

        explicit ItemInfoRequestPacket ( const json& data ) :
                network::IPacket( data ),
                _uri{ data[ "uri" ].get< std::string >() }              {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "uri" ] = uri().toString();
            return data;
        }

    property( uri, URI, public_const, public_ptr );
	};
}