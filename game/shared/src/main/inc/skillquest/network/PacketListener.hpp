/**
 * @author omnisudo
 * @date 2023.10.07
 */

#pragma once

#include <typeindex>
#include "skillquest/network/connection/Connection.hpp"
#include "Packet.hpp"
#include <functional>
#include <utility>
#include "skillquest/util/demangle.hpp"

namespace skillquest::network {
	class Channel;
	
	struct PacketListener {
		void* subject;
		
		std::string type;
		
		std::function< void ( network::Connection, Packet ) > callback;
		
		std::function< bool ( network::Connection, Packet ) > filter = [] (
				auto connection, auto packet
		) { return true; };
	
	public:
		PacketListener (
				std::type_index type,
				std::function< void ( network::Connection, Packet ) > callback,
				void* subject = nullptr
		) :
				subject( subject ),
				type( util::demangle( type.name() ) ),
				callback( std::move(callback) ) {
		}
	};
}