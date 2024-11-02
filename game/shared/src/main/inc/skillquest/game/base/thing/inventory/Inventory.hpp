/**
* @author omnisudo
 * @date 2024.09.14
 */

#pragma once

#include "skillquest/stuff.thing.hpp"
#include "skillquest/property.hpp"
#include "skillquest/item.hpp"

namespace skillquest::game::base::thing::inventory {
    class Inventory : public stuff::Thing {
        property(stacks, std::map< URI COMMA std::shared_ptr< thing::item::ItemStack > >, public_ref, none);

    public:
        struct CreateInfo {
            URI uri;
            std::map</* inventory slot */ URI, /* itemstack */ URI> stacks;
        };

        Inventory(const CreateInfo &info)
            : Thing({
                .uri = info.uri
            }) {
            for (auto &[slot, stack]: info.stacks) {
                this->_stacks[slot.toString()] = std::dynamic_pointer_cast< thing::item::ItemStack >(stuff()[stack]);
            }
        }

    public:
        virtual std::shared_ptr<thing::item::ItemStack> &operator [](const URI &uri) {
            return _stacks[uri.toString()];
        }
    };
}
