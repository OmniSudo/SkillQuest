/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/property.hpp"
#include "skillquest/string.hpp"
#include "skillquest/json.hpp"
#include "../../Packet.hpp"


namespace skillquest::network::packet::handshake {
	class DisconnectedPacket : public network::IPacket {
	public:
		DisconnectedPacket () : IPacket_INIT {
		}
		
		explicit DisconnectedPacket ( json data ) : network::IPacket( data ) {}
	
	public:
		json serialize () const override {
			return IPacket::serialize();
		}
	};
}
