#include "skillquest/network/database/NetworkClientDatabase.hpp"

#include <utility>

namespace skillquest::network::database {
	NetworkClientDatabase::NetworkClientDatabase (
			network::NetworkController* controller
	) :
			_controller( controller ) {
	}
	
	auto NetworkClientDatabase::connection (
			std::shared_ptr< network::connection::ClientConnection > connection
	) -> std::shared_ptr< network::connection::ClientConnection > {
		auto i = _clients.find( connection->address() );
		if ( i != _clients.end() ) return i->second;
		return ( _clients[ connection->address() ] = connection );
	}
	
	auto NetworkClientDatabase::connection (
			network::Address address
	) -> std::shared_ptr< network::connection::ClientConnection > {
		auto i = _clients.find( address );
		if ( i == _clients.end() ) return nullptr;
		return i->second;
	}
}