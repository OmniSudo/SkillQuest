#pragma once

#include "skillquest/network.hpp"

namespace skillquest::network::socket {
	class NetworkController;
	
	namespace database {
		/**
		* A linux socket client database
		* @author  OmniSudo
		* @date    12/6/21
		*/
		class NetworkClientDatabase : public skillquest::network::database::NetworkClientDatabase {
			std::mutex _mut;
		
		public:
			/**
			 * CTOR
			 * @param controller The networking controller
			 * @param logger What to use when logging information
			 */
			NetworkClientDatabase (
					network::socket::NetworkController* controller
			);
		
		public:
			/**
			 * @inherit
			 */
			skillquest::network::Connection connect (
					skillquest::network::Address address,
					const std::string& email,
					const std::string& password_hash,
					bool signup
			) override;
			
		};
	}
}
