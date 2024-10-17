#pragma once

#include "skillquest/network.hpp"
#include "ClientConnection.hpp"
#include <vector>
#include <memory>

namespace skillquest::network::winsock {
	class NetworkController;
	
	namespace connection {
		/**
		 *
		 * @author  OmniSudo
		 * @date    12/6/21
		 */
		class ServerConnection : public skillquest::network::connection::ServerConnection {
			void* _listen;
			
			std::map< skillquest::network::Address, skillquest::network::Connection > _clients;
			
			std::thread _accepting;
		
		public:
			
			/**
			 * CTOR
			 * @param module
			 * @param address
			 */
			ServerConnection (
					network::winsock::NetworkController* controller,
					skillquest::network::Address address
			);
			
			/**
			 * DTOR
			 */
			~ServerConnection () override;
		
		public:
			/**
			 * Disconnect from the servers
			 */
			void disconnect () override;
			
			/**
			 * Accept an incoming credentials
			 */
			void accept ();
			
		};
	}
}
