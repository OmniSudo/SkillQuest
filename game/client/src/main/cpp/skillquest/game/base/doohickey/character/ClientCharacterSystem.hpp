/**
 * @author  omnisudo
 * @date    2024.11.03
 */

#pragma once

#include "skillquest/property.hpp"
#include "skillquest/game/base/doohickey/character/ClientCharacterInventoryNetworking.hpp"

namespace skillquest::game::base::doohickey::character {
    class ClientCharacterSystem : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/character/inventory" };

        struct CreateInfo {
        };

        explicit ClientCharacterSystem( const CreateInfo& info );

        ~ClientCharacterSystem () override;

        property(inventories, std::shared_ptr< character::ClientCharacterInventoryNetworking >, public_ref, none);

    };
}