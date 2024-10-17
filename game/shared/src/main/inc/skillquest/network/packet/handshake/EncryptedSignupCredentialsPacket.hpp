/**
 * @author omnisudo
 * @date 2024.07.29
 */

#pragma once

#include "skillquest/property.hpp"
#include "skillquest/string.hpp"
#include "skillquest/json.hpp"
#include "../../Packet.hpp"

namespace skillquest::network::packet::handshake {
	class EncryptedSignupCredentialsPacket : public network::IPacket {
	property( password_hash, std::string, public_const, none )
	
	property( email, std::string, public_const, none )
	
	public:
		EncryptedSignupCredentialsPacket ( std::string email, std::string password_hash ) :
				_password_hash( password_hash ),
				_email( email ),
				IPacket_INIT {
		}
		
		explicit EncryptedSignupCredentialsPacket ( json data ) :
				network::IPacket( data ),
				_email{ data[ "e" ].get< std::string >() },
				_password_hash{ data[ "p" ].get< std::string >() } {
		}
	
	public:
		json serialize () const override {
			auto json = IPacket::serialize();
			json[ "p" ] = password_hash();
			json[ "e" ] = email();
			return json;
		}
	};
}
