/**
 * @author  omnisudo
 * @date    2024.11.02
 */

#pragma once

#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/network.hpp"
#include "skillquest/game/base/packet/inventory/character/CharacterInventoryInfoPacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"

namespace skillquest::game::base::doohickey::character {
    class ClientCharacterInventoryNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/character/inventory" };

        struct CreateInfo {
        };

        explicit ClientCharacterInventoryNetworking( const CreateInfo& info );

        ~ClientCharacterInventoryNetworking () override;

    private:
        net_receive( CharacterInventoryInfoPacket, packet::inventory::character::CharacterInventoryInfoPacket );

        property( channel, skillquest::network::Channel*&, protected_ref, none );
    };
}
