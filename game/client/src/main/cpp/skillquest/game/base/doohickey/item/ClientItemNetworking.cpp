/**
 * @author omnisudo
 * @date 2024.08.07
 */

#include "ClientItemNetworking.hpp"
#include "skillquest/game/base/thing/item/ClientItem.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::doohickey::item {
    ClientItemNetworking::ClientItemNetworking ( const ClientItemNetworking::CreateInfo& info )
            : stuff::Doohickey{ { .uri = CL_URI } },
              _channel{ sq::shared()->network()->channels().create( "item", true ) },
              _localplayer{ info.localplayer } {
        sq::shared()->network()->packets().add< packet::item::ItemInfoPacket >();
        _channel->add( this, &ClientItemNetworking::onNet_ItemInfoPacket );
    }

    void item::ClientItemNetworking::onNet_ItemInfoPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::item::ItemInfoPacket > data
    ) {
        // Create or update the item in the client's item storage
        sq::shared()->logger()->trace( "Creating item {0}", data->uri() );
        auto item = createOrUpdateItem( data->uri() );
        if ( item ) {
            sq::shared()->logger()->info( "Created item {0}", item->uri() );
            auto i = callbacks().find( data->uri() );
            if ( i != callbacks().end() ) {
                i->second.operator ()( item );
                callbacks().erase( i );
            }
        } else {
            sq::shared()->logger()->info( "Failed to create item {0}", data->uri() );
        }
    }

    void
    ClientItemNetworking::request ( const URI& uri, std::function< void ( std::shared_ptr< thing::item::IItem > item ) > callback ) {
        if ( callbacks().contains( uri ) ) {
            auto chain = callbacks()[ uri ];
            callbacks()[ uri ] = [ callback, chain, this ] ( std::shared_ptr< thing::item::IItem > item ) {
                chain( item );
                callback( item );
            };
        } else {
            callbacks()[ uri ] = callback;
        }

        // TODO: GET EXTENSION URI
        // _channel->send( localplayer()->connection(), new packet::item::ItemInfoRequestPacket{ uri } );
    }

    void ClientItemNetworking::onDeactivate () {
        Thing::onDeactivate();
        _channel->drop( this );
    }

    std::shared_ptr< thing::item::IItem > ClientItemNetworking::createOrUpdateItem ( const URI& uri ) {
        if ( std::dynamic_pointer_cast< thing::item::ClientItem >(
                stuff().contains( uri ) ? stuff()[ uri ] : nullptr
        ) ) {
            stuff().remove( uri );
        }

        return stuff().create< thing::item::ClientItem >( { .uri = uri } );
    }
}// namespace skillquest::game::base::thing::item
