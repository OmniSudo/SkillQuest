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
    class InventorySyncPacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * Sends inventory information to client
         * @param name Character to log into
         */
        explicit InventorySyncPacket(sq::sh::Inventory &target)
            : IPacket_INIT, _target{target->uri()} {
            for ( const auto& [ slot, stack ] : target->stacks() ) {
                _stacks[ slot ] = stack->uri().path();
            }
        }

        explicit InventorySyncPacket(const json &data)
            : network::IPacket(data), _target{ data[ "uri" ].get< std::string >() } {
            auto stacks = data[ "stacks" ];
            for ( auto pair = stacks.begin(); pair != stacks.end(); ++pair ) {
                _stacks[ { pair.key() } ] = { pair.value().get< std::string >() };
            }
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target.toString();
            auto stacks = data["stacks"] = {};
            for (const auto &[ uri, stack ] : _stacks) {
                stacks[ uri.toString() ] = stack.toString();
            }
            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(stacks, std::map< URI COMMA util::UID >, public_const, public_ptr );
    };
};
