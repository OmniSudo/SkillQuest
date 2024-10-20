/**
 * @author omnisudo
 * @date 2024.09.02
 */

#include "skillquest/item.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::item {
    class ItemInfoDeniedPacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * TODO: item properties to packet properties
		 * @param item
		 */
        explicit ItemInfoDeniedPacket ( const std::shared_ptr<thing::item::IItem>& item ) :
                IPacket_INIT,
                _uri{ item->uri() } {
        }

        explicit ItemInfoDeniedPacket ( const json& data ) :
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