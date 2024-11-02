/**
 * @author  omnisudo
 * @date    2024.11.02
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryInitDeniedPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventoryInitDeniedPacket(const URI& inventory)
            : IPacket_INIT, _target{inventory} {
        }

        explicit InventoryInitDeniedPacket(const json &data)
            : network::IPacket(data ),
        _target{ data[ "uri" ].get< std::string >() }
        {
        }

        json serialize() const override {
          json data = IPacket::serialize();
            data["uri"] = _target.toString();
            return data;
        }

        property(target, URI, public_const, public_ptr);
    };
};

