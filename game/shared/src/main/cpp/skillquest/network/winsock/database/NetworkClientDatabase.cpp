/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#ifdef PLATFORM_WINDOWS

#include "skillquest/network/winsock/database/NetworkClientDatabase.hpp"
#include "skillquest/network/winsock/connection/ClientConnection.hpp"
#include "skillquest/network/winsock/Controller.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::winsock::database {
	NetworkClientDatabase::NetworkClientDatabase(
			winsock::NetworkController *controller
	) : skillquest::network::database::NetworkClientDatabase(controller) {
	}

	skillquest::network::Connection NetworkClientDatabase::connect(skillquest::network::Address address) {
		auto socketController = dynamic_cast<winsock::NetworkController *>(_controller);
		auto client = _clients[ address ] = skillquest::network::Connection(
				new winsock::connection::ClientConnection(
						socketController,
						address
				),
				[this](connection::ClientConnection *ptr) {
					auto i = _clients.find( ptr->address() );
					if ( i != _clients.end() ) {
						sq::shared()->logger()->trace( "Erasing client credentials @ {0}", ptr->address() );
						_clients.erase( i );
					}
					delete ptr;
				}
		);
		client->connect();
		return client;
	}
}

#endif