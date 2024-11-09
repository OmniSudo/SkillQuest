/**
 * @author  omnisudo
 * @date    2024.11.03
 */

#pragma once

#include "skillquest/uri.hpp"
#include <memory>
#include "skillquest/stuff.doohickey.hpp"

#include "skillquest/game/base/doohickey/block/ClientBlockNetworking.hpp"
#include "skillquest/game/base/doohickey/character/ClientCharacterInventoryNetworking.hpp"
#include "skillquest/game/base/doohickey/character/ClientCharacterSystem.hpp"
#include "skillquest/game/base/doohickey/item/ClientItemNetworking.hpp"
#include "skillquest/game/base/doohickey/item/ClientItemStackNetworking.hpp"
#include "skillquest/game/base/doohickey/ui/UIDrawable.hpp"
#include "skillquest/game/base/thing/world/ClientWorld.hpp"

namespace skillquest::game::base::doohickey::addon {
    class BaseAddonCL : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "cl://addon.skill.quest/base" };

        static std::shared_ptr< BaseAddonCL > instance ();

        struct CreateInfo {
            sq::cl::LocalPlayer player;
        };

        explicit BaseAddonCL( const CreateInfo& info );

        ~BaseAddonCL () override;

        void onActivate () override;

        void onDeactivate () override;

        std::shared_ptr<thing::world::ClientWorld> world();

        auto world( std::shared_ptr< thing::world::ClientWorld > world ) -> void;

    private:
        property(items, std::shared_ptr< item::ClientItemNetworking >, public_ref, none);
        property(itemstacks, std::shared_ptr< item::ClientItemStackNetworking >, public_ref, none);
        property(blocks, std::shared_ptr< block::ClientBlockNetworking >, public_ref, none);
        property(characters, std::shared_ptr< character::ClientCharacterSystem >, public_ref, none );

        property(player, sq::cl::LocalPlayer, public, none );

    };
}
