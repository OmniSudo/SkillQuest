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
    class CharacterInventoryRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit CharacterInventoryRequestPacket(sq::sh::Character character)
            : IPacket_INIT, _character{character->uri()} {
        }

        explicit CharacterInventoryRequestPacket(const json &data)
            : network::IPacket(data),
              _character{data["character"].get<std::string>()} {
        }

        json serialize() const override {
            json data = IPacket::serialize();

            data["character"] = _character.toString();

            return data;
        }

        property(character, URI, public_const, public_ptr);
    };
};
