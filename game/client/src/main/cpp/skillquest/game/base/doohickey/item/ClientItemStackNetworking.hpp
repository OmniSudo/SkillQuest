/**
 * @author omnisudo
 * @date 2024.08.08
 */

#pragma once

#include "skillquest/game/base/packet/item/ItemStackInfoPacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/network.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::doohickey::item {
    class ClientItemStackNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/itemstack" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientItemStackNetworking( const CreateInfo& info );

        void onDeactivate() override;

    private:
        std::shared_ptr< thing::item::ItemStack > createOrUpdateItemStack(
                const util::UID& uid,
                const URI& item_uri,
                const util::UID& owner,
                std::size_t count );

        net_receive( ItemStackInfoPacket, packet::item::ItemStackInfoPacket );

        property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
        property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
        property( callbacks, std::map< util::UID COMMA std::function< void ( std::shared_ptr< thing::item::ItemStack > stack ) > >, protected_ref, none );
    };
}// namespace skillquest::game::base::thing::item
