/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include "skillquest/network/socket/database/NetworkClientDatabase.hpp"
#include "skillquest/network/socket/connection/ClientConnection.hpp"
#include "skillquest/network/socket/Controller.hpp"

namespace skillquest::network::socket::database {
	NetworkClientDatabase::NetworkClientDatabase (
			socket::NetworkController* controller
	) : skillquest::network::database::NetworkClientDatabase( controller ) {
	}
	
	skillquest::network::Connection NetworkClientDatabase::connect (
			skillquest::network::Address address,
			const std::string& email,
			const std::string& password_hash,
			bool signup
	) {
		auto socketController = dynamic_cast<socket::NetworkController*>(_controller);
		auto i = _clients.find( address );
		
		if ( i != _clients.end() ) {
			auto connection = i->second;
			_clients.erase( i );
			connection->disconnect();
		}
		
		auto client = _clients[ address ] = std::make_shared< socket::connection::ClientConnection >(
				socketController,
				address
		);
		
		client->connect( email, password_hash, signup );
		return client;
	}
}

#endif