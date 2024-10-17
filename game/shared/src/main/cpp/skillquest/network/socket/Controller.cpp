/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/network/socket/Controller.hpp"
#include "skillquest/network/socket/database/NetworkClientDatabase.hpp"
#include "skillquest/network/socket/database/NetworkServerDatabase.hpp"

#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include <csignal>

namespace skillquest::network::socket {
	NetworkController::NetworkController () : skillquest::network::NetworkController() {
		_clients = new network::socket::database::NetworkClientDatabase( this );
		_servers = new network::socket::database::NetworkServerDatabase( this );
	}
	
	NetworkController::~NetworkController () {}
}
#endif