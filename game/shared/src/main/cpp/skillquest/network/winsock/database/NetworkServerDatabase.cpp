/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#ifdef PLATFORM_WINDOWS

#include "skillquest/network/winsock/database/NetworkServerDatabase.hpp"
#include "skillquest/network.hpp"
#include "skillquest/network/winsock/connection/ServerConnection.hpp"
#include "skillquest/network/winsock/Controller.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::winsock::database {
	NetworkServerDatabase::NetworkServerDatabase (
			network::winsock::NetworkController* controller
			)
			: skillquest::network::database::NetworkServerDatabase( controller ) {
		
	}
	
	auto NetworkServerDatabase::open (
			const skillquest::network::Address& address
	) -> std::shared_ptr< skillquest::network::connection::ServerConnection > {
		return _servers[ address ] = std::shared_ptr< skillquest::network::connection::ServerConnection >(
				new connection::ServerConnection(
						dynamic_cast<winsock::NetworkController*>(_controller),
						address
				),
				[ this ] ( skillquest::network::connection::ServerConnection* ptr ) {
					auto i = _servers.find( ptr->address() );
					if ( i != _servers.end() ) {
						sq::shared()->logger()->trace( "Erasing server credentials @ {0} connections: {1}",
											  ptr->address(), ptr->connections().size() );
						_servers.erase( i );
					}
					delete ptr;
				}
		);
	}
}

#endif