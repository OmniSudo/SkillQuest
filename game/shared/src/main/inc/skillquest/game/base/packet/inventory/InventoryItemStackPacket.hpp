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
        explicit InventoryItemStackPacket(sq::sh::Inventory target, const URI &slot)
            : IPacket_INIT, _target{target->uri()},
              _stack{target->stacks()[slot] ? target->stacks()[slot]->uri() : {}} {
        }

        explicit InventoryItemStackPacket(const json &data)
            : network::IPacket(data),
              _target{data["uri"].get<std::string>()},
              _stack{data["stack"].get<std::string>()} {
            if (!_target) {
                _target = std::make_shared<thing::inventory::Inventory>(sq::sh::Inventory::element_type::CreateInfo{});
            }
        }

        json serialize() const override {
            json data = IPacket::serialize();
            if (_target) {
                data["uri"] = _target.toString();
                data["stack"] = _stack.toString();
            }
            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(stack, URI, public_const, public_ptr);
    };
};
