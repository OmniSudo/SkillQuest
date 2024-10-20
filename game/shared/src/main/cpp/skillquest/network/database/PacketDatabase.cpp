/**
 * @author omnisudo
 * @date 2023.07.21
 */

#include "skillquest/network/database/PacketDatabase.hpp"
#include "skillquest/network/NetworkController.hpp"
#include <utility>
#include "skillquest/network/connection/Connection.hpp"
#include "skillquest/util/convert.hpp"

#include "skillquest/sh.api.hpp"

namespace skillquest::network::database {
	PacketDatabase::PacketDatabase (
			network::NetworkController* controller
	) : _controller( controller ) {
	
	}
	
	auto PacketDatabase::get ( std::type_index type, json packet ) -> network::Packet {
		return get( util::demangle( type ), packet );
	}
	
	auto PacketDatabase::get ( std::string type, json json ) -> network::Packet {
		json[ "type" ] = type;
		return get( json );
	}
	
	auto PacketDatabase::get ( json json ) -> std::shared_ptr< network::IPacket > {
		if ( json.empty() || !json.contains( "type" ) ) return nullptr;
		std::scoped_lock lock( _mutex );
		
		auto type = json[ "type" ].get< std::string >();
		auto i = _packetDeserializers.find( type );
		if ( i != _packetDeserializers.end() ) {
			auto packet = i->second.operator()( json );
	 		return packet;
		}
		
		return nullptr;
	}
	
	
}