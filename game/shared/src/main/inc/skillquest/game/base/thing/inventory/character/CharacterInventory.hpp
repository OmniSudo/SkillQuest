/**
 * @author omnisudo
 * @date 2024.09.14
 */

#pragma once

#include "skillquest/stuff.thing.hpp"
#include "skillquest/property.hpp"
#include "skillquest/item.hpp"

namespace skillquest::game::base::thing {
    namespace character {
        class Character;
    }

    namespace inventory::character {
        class CharacterInventory : stuff::Thing {
            property( stacks, std::map< URI COMMA std::shared_ptr< thing::item::ItemStack > >, public_ref, none );
            property( character, std::shared_ptr< thing::character::Character >, public, none );

        public:
            virtual std::shared_ptr< thing::item::ItemStack >& operator [] ( const URI& uri ) {
                return _stacks[ uri ];
            }
        };

    }
}