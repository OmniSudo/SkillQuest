/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include "skillquest/network/socket/database/NetworkServerDatabase.hpp"
#include "skillquest/network.hpp"
#include "skillquest/network/socket/connection/ServerConnection.hpp"
#include "skillquest/network/socket/Controller.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::socket::database {
	NetworkServerDatabase::NetworkServerDatabase (
			network::socket::NetworkController* controller
	)
			: skillquest::network::database::NetworkServerDatabase( controller ) {
		
	}
	
	auto NetworkServerDatabase::open (
			const skillquest::network::Address& address
	) -> std::shared_ptr< skillquest::network::connection::ServerConnection > {
		return _servers[ address ] = std::shared_ptr< skillquest::network::connection::ServerConnection >(
				new connection::ServerConnection(
						dynamic_cast<socket::NetworkController*>(_controller),
						address
				),
				[ this ] ( skillquest::network::connection::ServerConnection* ptr ) {
					auto i = _servers.find( ptr->address() );
					if ( i != _servers.end() ) {
						sq::shared()->logger()->trace(
								"Erasing server credentials @ address:{0} address:{1} connections: {2}",
								ptr->address(), ptr->address(), ptr->connections().size() );
						_servers.erase( i );
					}
					delete ptr;
				}
		);
	}
}

#endif