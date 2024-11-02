/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/inventory.hpp"
#include "skillquest/item.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryItemStackPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventoryItemStackPacket(sq::sh::Inventory target, const URI& uri)
            : IPacket_INIT, _target{target->uri()}, _slot{uri}, _stack{ "itemstack://skill.quest/null" } {
            auto stack = target->stacks().contains(uri) ? target->stacks()[uri] : nullptr;
            _stack = stack ? stack->uri() : _stack;
        }

        explicit InventoryItemStackPacket(const json &data)
            : network::IPacket(data),
              _target{data["uri"].get<std::string>()},
              _slot{data["slot"].get<std::string>()},
              _stack{data["stack"].get<std::string>()} {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target.toString();
            data["slot"] = _slot.toString();
            data["stack"] = _stack.toString();
            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(slot, URI, public_const, public_ptr );
        property(stack, URI, public_const, public_ptr);
    };
};
