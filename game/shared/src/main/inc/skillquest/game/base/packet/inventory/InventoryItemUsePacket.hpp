/**
 * @author  omnisudo
 * @date    2024.10.18
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryItemUsePacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * Tells the client that an item has been used
         * @param target
         */
        explicit InventoryItemUsePacket(sq::sh::Inventory &target, const URI &slot)
            : IPacket_INIT, _target{target->uri()}, _slot{slot} {
        }

        explicit InventoryItemUsePacket(const json &data)
            : network::IPacket(data),
              _target{                          data["uri"].get<std::string>()              },
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
