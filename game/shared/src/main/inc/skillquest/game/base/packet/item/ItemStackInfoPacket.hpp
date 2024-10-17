/**
 * @author omnisudo
 * @date 8/7/24
 */
#pragma once

#include "skillquest/item.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::item {
    class ItemStackInfoPacket : public network::IPacket {
    public:
        /**
		 * SERVER
		 * TODO: item properties to packet properties
		 * @param item
		 */
        explicit ItemStackInfoPacket( std::shared_ptr< thing::item::ItemStack > itemstack )
            : IPacket_INIT,
              _item_uri{ itemstack->item()->uri() },
              _owner_uid{ itemstack->owner()->uid() },
              _count{ itemstack->count() },
              _stack_uid{ itemstack->uri().path() } {
        }

        explicit ItemStackInfoPacket( const json& data )
            : network::IPacket( data ),
              _item_uri{ data[ "item" ].get< std::string >() },
              //_count{ data[ "count" ].get< std::size_t >() },
              _owner_uid{ data[ "owner" ].get< std::string >() },
              _stack_uid{ data[ "stack" ].get< std::string >() } {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data[ "item" ] = item_uri().toString();
            data[ "stack" ] = stack_uid().toString();
            data[ "count" ] = count();
            data[ "owner" ] = owner_uid().toString();
            return data;
        }

        property( stack_uid, util::UID, public_const, public_ptr );
        property( owner_uid, util::UID, public_const, public_ptr );
        property( item_uri, URI, public_const, public_ptr );
        property( count, std::size_t, public_const, public_ptr );
    };
}// namespace skillquest::game::base::packet::item