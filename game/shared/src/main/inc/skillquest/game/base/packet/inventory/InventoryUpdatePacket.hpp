/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include "skillquest/inventory.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/thing/inventory/Inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryUpdatePacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventoryUpdatePacket(const sq::sh::Inventory &target, const URI &slot)
            : IPacket_INIT, _target{target->uri()}, _slot{slot} {
        }

        explicit InventoryUpdatePacket(const json &data)
            : network::IPacket(data),
              _target{data["uri"].get<std::string>()},
              _slot{data["slot"].get<std::string>()} {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target.toString();
            data["slot"] = _slot.toString();
            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(slot, URI, public_const, public_ptr);
    };
};
