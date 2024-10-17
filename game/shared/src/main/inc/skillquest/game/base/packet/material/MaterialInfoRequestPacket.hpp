/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/network/Packet.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::material {
	class MaterialInfoRequestPacket : public network::IPacket {
	public:
		explicit MaterialInfoRequestPacket ( const URI& uri ) : IPacket_INIT, _material_uri{ uri } {
		
		}
		
		explicit MaterialInfoRequestPacket ( const json& data ) :
				network::IPacket( data ),
				_material_uri{ data[ "uri" ].get< std::string >() } {
		}
		
		json serialize () const override {
			json data = IPacket::serialize();
			auto uri = material_uri().toString();
			if ( !uri.empty() ) {
				data[ "uri" ] = uri;
			}
			return data;
		}
	
	property( material_uri, URI, public_const, none );
	};
}