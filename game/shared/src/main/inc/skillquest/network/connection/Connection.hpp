/**
 * @author omnisudo
 * @date 2023.07.21
 */

#pragma once

#include <memory>

namespace skillquest::network {
	namespace connection {
		class ClientConnection;
	}
	
	typedef std::shared_ptr< network::connection::ClientConnection > Connection;
}