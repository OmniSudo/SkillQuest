/**
 * @author  omnisudo
 * @date    2024.10.18
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"
#include "skillquest/character.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/thing/inventory/character/CharacterInventory.hpp"

namespace skillquest::game::base::packet::inventory {
    class InventorySyncRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         * Requests the server to send inventory information
         * @param target
         */
        explicit InventorySyncRequestPacket(std::shared_ptr<thing::inventory::Inventory> &target)
            : IPacket_INIT, _target{target} {
        }

        explicit InventorySyncRequestPacket(const json &data)
            : network::IPacket(data),
              _target{
                  std::dynamic_pointer_cast<thing::inventory::Inventory>(
                      sq::shared()->stuff()[
                          data["uri"].get<std::string>()
                      ]
                  )
              } {
            if (!_target) {
                _target = std::make_shared<thing::inventory::Inventory>({});
            }
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uri"] = _target->uri().toString();
            return data;
        }

        property(target, std::shared_ptr<thing::inventory::Inventory>, public_const, public_ptr);
    };
};
