/**
 * @author omnisudo
 * @date 2024.08.08
 */

#include "ClientItemStackNetworking.hpp"
#include "skillquest/game/base/thing/item/ClientItem.hpp"
#include "ClientItemNetworking.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::doohickey::item {
    ClientItemStackNetworking::ClientItemStackNetworking( const CreateInfo& info )
        : stuff::Doohickey{ { .uri = CL_URI } },
    _channel{ sq::shared()->network()->channels().create( "itemstack" ) }{
        sq::shared()->network()->packets().add< packet::item::ItemStackInfoPacket >();
        _channel->add( this, &ClientItemStackNetworking::onNet_ItemStackInfoPacket );
    }

    void ClientItemStackNetworking::onDeactivate() {
        Thing::onDeactivate();
        _channel->drop( this );
    }

    std::shared_ptr< thing::item::ItemStack > ClientItemStackNetworking::createOrUpdateItemStack(
            const util::UID& uid,
            const URI& item_uri,
            const util::UID& owner,
            std::size_t count ) {
        auto uri = URI{ "itemstack://skill.quest/" + uid.toString() };
        auto itemstack = std::dynamic_pointer_cast< thing::item::ItemStack >( stuff().contains( uri ) ? stuff()[ uri ] : nullptr );
        auto item = std::dynamic_pointer_cast< thing::item::IItem >( stuff().contains( item_uri ) ? stuff()[ item_uri ] : nullptr );
        if( !itemstack ) {
            itemstack = stuff().create< thing::item::ItemStack >( { .id = uid,
                                                       .owner = localplayer(),// TODO: Allow for other character UIDs to own stacks
                                                       .item = item,
                                                       .count = count } );
            if( !item ) {
                std::dynamic_pointer_cast< ClientItemNetworking >( stuff()[ ClientItemNetworking::CL_URI ] )->request( item_uri, [ itemstack ]( auto item ) {
                    itemstack->item( item );
                } );
            }
        }

        return itemstack;
    }

    void ClientItemStackNetworking::onNet_ItemStackInfoPacket( network::Connection connection, std::shared_ptr< packet::item::ItemStackInfoPacket > data ) {
        // Create or update the item in the client's item storage
        sq::shared()->logger()->trace( "Downloaded item stack {0}", data->stack_uid() );
        auto stack = createOrUpdateItemStack(
                data->stack_uid(),
                data->item_uri(),
                data->owner_uid(),
                data->count() );
        sq::shared()->logger()->info( "Created item stack {0}", data->stack_uid() );
    }
};// namespace skillquest::game::base::thing::item
