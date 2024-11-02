/**
 * @author  omnisudo
 * @date    2024.10.18
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryItemUseRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Sends inventory information to client
         * @param name Character to log into
         */
        explicit InventoryItemUseRequestPacket(sq::sh::Inventory target, const URI& slot )
            : IPacket_INIT, _target{target->uri()}, _slot{ slot } {
        }

        explicit InventoryItemUseRequestPacket(const json &data)
            : network::IPacket(data),
        _target{ data["uri"].get<std::string>()},
        _slot{ data[ "slot" ].get< std::string >() } {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target.toString();
            data[ "slot" ] = _slot.toString();
            return data;
        }

        property(target, URI, public, public_ptr);
        property(slot, URI, public_const, public_ptr);
    };
};
