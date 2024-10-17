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
	class EncryptedCredentialsPacket : public network::IPacket {
	property( email, std::string, public_const, none )
	
	property( password_hash, std::string, public_const, none )
	
	public:
		EncryptedCredentialsPacket ( std::string email, std::string password_hash ) :
				_email( email ),
				_password_hash( password_hash ),
				IPacket_INIT {
		}
		
		explicit EncryptedCredentialsPacket ( json data ) :
				network::IPacket( data ),
				_email{ data[ "e" ].get< std::string >() },
				_password_hash{ data[ "p" ].get< std::string >() } {
		}
	
	public:
		json serialize () const override {
			auto json = IPacket::serialize();
			json[ "e" ] = email();
			json[ "p" ] = password_hash();
			return json;
		}
	};
}
