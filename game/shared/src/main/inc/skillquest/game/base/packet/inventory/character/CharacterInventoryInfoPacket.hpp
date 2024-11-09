/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/inventory.hpp"
#include "skillquest/character.hpp"
#include "skillquest/item.hpp"

namespace skillquest::game::base::packet::inventory::character {
    class CharacterInventoryInfoPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit CharacterInventoryInfoPacket(sq::sh::Character character)
            : IPacket_INIT, _character{character->uri()}, _inventory{character->inventory() ? character->inventory()->uri() : URI{"inventory://skill.quest/null" } } {
        }

        explicit CharacterInventoryInfoPacket(const json &data)
            : network::IPacket(data),
              _character{data["character"].get<std::string>()},
              _inventory{data["inventory"].get<std::string>()} {
        }

        json serialize() const override {
            json data = IPacket::serialize();

            data["character"] = _character.toString();
            data["inventory"] = _inventory.toString();

            return data;
        }

        property(character, URI, public_const, public_ptr);
        property(inventory, URI, public_const, public_ptr);
    };
};
