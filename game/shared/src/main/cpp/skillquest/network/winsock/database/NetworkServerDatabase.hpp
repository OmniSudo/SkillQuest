#pragma once

#include "skillquest/network.hpp"

namespace skillquest::network::winsock {
	class NetworkController;
	
	namespace database {
		/**
		* An ASIO Network servers database
		* @author  OmniSudo
		* @date    12/6/21
		*/
		class NetworkServerDatabase : public skillquest::network::database::NetworkServerDatabase {
		public:
			NetworkServerDatabase (
					network::winsock::NetworkController* controller
			);
		
		public:
			auto open (
					const skillquest::network::Address& address
			) -> std::shared_ptr <skillquest::network::connection::ServerConnection> override;
			
		};
	}
}