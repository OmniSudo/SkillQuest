#include "skillquest/network/connection/ServerConnection.hpp"

#include "skillquest/network/NetworkController.hpp"

namespace skillquest::network::connection {
	ServerConnection::~ServerConnection () {
		auto i = controller()->servers()._servers.find( address() );
		if ( i != controller()->servers()._servers.end() ) {
			controller()->servers()._servers.erase( i );
		}
	}
}