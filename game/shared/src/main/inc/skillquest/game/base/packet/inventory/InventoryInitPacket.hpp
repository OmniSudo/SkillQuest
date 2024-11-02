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
    class InventoryInitPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventoryInitPacket(sq::sh::Inventory &target)
            : IPacket_INIT, _target{target->uri()}, _stacks{} {
            for ( const auto&[slot, stack] : target->stacks() ) {
                _stacks.emplace( slot, stack->uri() );
            }
        }

        explicit InventoryInitPacket(const json &data)
            : network::IPacket(data ),
        _target{ data[ "uri" ].get< std::string >() }
        {
            auto slots = data[ "slots" ];
            for ( auto pair = slots.begin(); pair != slots.end(); pair++ ) {
                auto stack = URI{ pair.value().get< std::string >() };
                _stacks.emplace( URI{pair.key()            }, stack );
            }
        }

        json serialize() const override {
            json data = IPacket::serialize();

            data["uri"] = _target.toString();
            auto& stacks = data[ "stacks" ] = {};
            for ( const auto& [ slot, uri ] : this->stacks() ) {
                stacks[ slot.toString() ] = uri.toString();
            }

            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(stacks, std::map< URI COMMA URI >, public_const, public_ptr);
    };
};
