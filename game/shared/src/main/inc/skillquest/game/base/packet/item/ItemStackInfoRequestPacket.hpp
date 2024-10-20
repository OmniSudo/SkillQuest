/**
 * @author  omnisudo
 * @date    2024.10.19
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/uid.hpp"

namespace skillquest::game::base::packet::item {
    class ItemStackInfoRequestPacket : public network::IPacket {
    public:
        /**
         * CLIENT -> SERVER
         */
        explicit ItemStackInfoRequestPacket(const util::UID &uid) : IPacket_INIT, _uid{uid} {
        }

        explicit ItemStackInfoRequestPacket(const json &data) : network::IPacket(data),
                                                                _uid{data["uid"].get<std::string>()} {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data["uid"] = uid().toString();
            return data;
        }

        property(uid, util::UID, public_const, public_ptr);
    };
}
