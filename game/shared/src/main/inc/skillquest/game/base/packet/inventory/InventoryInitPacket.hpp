/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/inventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventoryInitPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventoryInitPacket(sq::sh::Inventory &target)
            : IPacket_INIT, _target{target->uri()} {
            for ( const auto& [ slot, stack ] : target->stacks() ) {
                _stacks[ slot ] = stack;
            }
        }

        explicit InventoryInitPacket(const json &data)
            : network::IPacket(data ),
        _target{ data[ "uri" ].get< std::string >() }
        {
            for ( const auto& [ slot, stack_json ] : data[ "slots"] ) {
                auto stack = URI{ stack_json.get< std::string >()            };
                _stacks[ URI{ slot } ] = stack;
            }
        }

        json serialize() const override {
            json data = IPacket::serialize();

            data["uri"] = _target.toString();
            auto& stacks = data[ "stacks" ] = {};
            for ( const auto& [ slot, uri ] : _target->stacks() ) {
                stacks[ slot.toString() ] = uri.toString();
            }

            return data;
        }

        property(target, URI, public_const, public_ptr);
        property(stacks, std::map< URI COMMA URI >, public_const, public_ptr);
    };
};
