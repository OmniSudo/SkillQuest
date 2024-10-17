/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include <utility>

#include "skillquest/game/base/packet/item/ItemInfoPacket.hpp"
#include "skillquest/game/base/packet/item/ItemInfoRequestPacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/network.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::doohickey::item {
    class ClientItemNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/item" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientItemNetworking ( const CreateInfo& info );

        void onDeactivate () override;

        void request ( const URI& uri, std::function< void ( std::shared_ptr< thing::item::IItem > item ) > callback );

    private:
        std::shared_ptr< thing::item::IItem > createOrUpdateItem ( const URI& uri );

        net_receive( ItemInfoPacket, packet::item::ItemInfoPacket );

    property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
    property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
    property( callbacks, std::map< URI COMMA std::function< void ( std::shared_ptr< thing::item::IItem > item ) > >, protected_ref,
              none );
    };
}// namespace skillquest::game::base::thing::item