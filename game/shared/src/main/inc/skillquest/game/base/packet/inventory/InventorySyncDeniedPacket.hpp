/**
 * @author  omnisudo
 * @date    2024.10.18
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"
#include "skillquest/inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventorySyncDeniedPacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * Sends inventory information to client
         * @param name Character to log into
         */
        explicit InventorySyncDeniedPacket(const URI &target)
            : IPacket_INIT, _target{target} {
        }

        explicit InventorySyncDeniedPacket(const json &data)
            : network::IPacket(data), _target{ data[ "uri" ].get< std::string >() } {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target.toString();
            return data;
        }

        property(target, URI, public_const, public_ptr);
    };
};
