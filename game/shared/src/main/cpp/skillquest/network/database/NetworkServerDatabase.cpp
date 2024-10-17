#include "skillquest/network/database/NetworkServerDatabase.hpp"

namespace skillquest::network::database {
	NetworkServerDatabase::NetworkServerDatabase (
			network::NetworkController* controller
	) :
			_controller( controller ) {
		
	}
}