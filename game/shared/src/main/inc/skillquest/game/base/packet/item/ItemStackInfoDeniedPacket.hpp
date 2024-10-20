/**
 * @author  omnisudo
 * @date    2024.10.19
 */

#pragma once

#include "skillquest/item.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::item {
    class ItemStackInfoDeniedPacket : public network::IPacket {
    public:
        /**
         * SERVER -> CLIENT
         * TODO: block properties to packet properties
         * @param stack
         */
        explicit ItemStackInfoDeniedPacket(const util::UID &uid) :
            IPacket_INIT,
            _uid{uid} {
        }

        explicit ItemStackInfoDeniedPacket(const json &data) : network::IPacket(data),
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
