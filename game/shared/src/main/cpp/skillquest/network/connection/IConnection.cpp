/**
 * @author omnisudo
 * @date 2023.08.23
 */

#include "skillquest/network/connection/IConnection.hpp"
#include "skillquest/network/NetworkController.hpp"

namespace skillquest::network::connection {
	IConnection::IConnection ( network::NetworkController* controller, network::Address address ) :
			_controller( controller ),
			_address( address ) {
		
	}
}