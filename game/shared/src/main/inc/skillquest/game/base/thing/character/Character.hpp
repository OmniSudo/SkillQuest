/**
 * @author omnisudo
 * @date 2024.07.26
 */

#pragma once

#include "skillquest/stuff.thing.hpp"
#include "skillquest/inventory.hpp"
#include "skillquest/util/uid.hpp"

namespace skillquest::game::base::thing::character {
	class Character : public stuff::Thing {
	public:
        struct CreateInfo {
            const stuff::Thing::CreateInfo& thing;
            util::UID uid;
        };

		Character ( const CreateInfo& info ) : stuff::Thing{ info.thing }, _uid{ info.uid } {}
		
		~Character () override = default;

        property( uid, util::UID, public, public_ptr )
        property(inventory, sq::sh::Inventory, public_ref, public_ptr);

	};
}