/**
 * @author  omnisudo
 * @date    2024.10.18
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"
#include "skillquest/inventory.hpp"
#include "skillquest/game/base/thing/inventory/character/CharacterInventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventorySyncPacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * Sends inventory information to client
         * @param name Character to log into
         */
        explicit InventorySyncPacket(sq::sh::Inventory &target)
            : IPacket_INIT, _target{target} {
        }

        explicit InventorySyncPacket(const json &data)
            : network::IPacket(data) {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target->uri().toString();
            auto stacks = data["stacks"] = {};
            for (const auto &[ uri, stack ] : _target->stacks()) {
                stacks[ uri.toString() ] = stack->uri().toString();
            }
            return data;
        }

        property(target, URI, public_const, public_ptr);
    };
};
