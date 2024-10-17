/**
 * @author omnisudo
 * @date 7/27/24
 */
#pragma once

#include "skillquest/property.hpp"
#include "skillquest/string.hpp"
#include "skillquest/json.hpp"
#include "../../Packet.hpp"

namespace skillquest::network::packet::handshake {
	class PublicKeyPacket : public network::IPacket {
	property( public_key, std::string, public_const, none )
	
	public:
		PublicKeyPacket ( std::string key ) :
				_public_key( key ),
				IPacket_INIT {
		}
		
		explicit PublicKeyPacket ( json data ) :
				network::IPacket( data ),
				_public_key{ data[ "k" ].get< std::string >() } {
		}
	
	public:
		json serialize () const override {
			auto json = IPacket::serialize();
			json[ "k" ] = public_key();
			return json;
		}
	};
}
