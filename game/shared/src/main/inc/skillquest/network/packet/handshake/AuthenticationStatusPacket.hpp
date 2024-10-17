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
	class AuthenticationStatusPacket : public network::IPacket {
	public:
	property( status, bool, public_const, none )
	
	property( reason, std::string, public_const, none )
	
	public:
		AuthenticationStatusPacket ( bool success, std::string reason = "" ) :
				_status{ success },
				_reason{ reason },
				IPacket_INIT {
		}
		
		explicit AuthenticationStatusPacket ( json data ) :
				network::IPacket( data ),
				_status{ data[ "s" ].get< bool >() },
				_reason{ data[ "r" ].get< std::string >() } {
			
		}
	
	public:
		json serialize () const override {
			auto json = IPacket::serialize();
			json[ "s" ] = _status;
			json[ "r" ] = _reason;
			return json;
		}
	};
}
