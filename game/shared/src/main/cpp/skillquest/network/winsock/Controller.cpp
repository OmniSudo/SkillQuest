/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#ifdef PLATFORM_WINDOWS

#include "skillquest/network/winsock/Controller.hpp"
#include "skillquest/network/winsock/database/NetworkClientDatabase.hpp"
#include "skillquest/network/winsock/database/NetworkServerDatabase.hpp"
#include "skillquest/sh.api.hpp"
#define _WINSOCKAPI_
#include <WinSock2.h>

namespace skillquest::network::winsock {
	NetworkController::NetworkController () : skillquest::network::NetworkController() {
		_wsaData = new ::WSAData{};
		int result = WSAStartup( MAKEWORD( 2, 2 ), reinterpret_cast< WSAData* >( _wsaData ) );

		if ( result != NO_ERROR ) {
			sq::shared()->logger()->error( "Failed to startup windows socket layer.\n{0}", result );
		}

		_clients = new network::winsock::database::NetworkClientDatabase( this );
		_servers = new network::winsock::database::NetworkServerDatabase( this );
	}
	
	NetworkController::~NetworkController () {
		WSACleanup();
		delete reinterpret_cast< WSAData* >( _wsaData );
		_wsaData = nullptr;
	}
}

#endif