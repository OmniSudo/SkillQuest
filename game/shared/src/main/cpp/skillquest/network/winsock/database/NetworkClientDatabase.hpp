#pragma once

#include "skillquest/network.hpp"

namespace skillquest::network::winsock {
	class NetworkController;
	
	namespace database {
		/**
		* A linux socket client database
		* @author  OmniSudo
		* @date    12/6/21
		*/
		class NetworkClientDatabase : public skillquest::network::database::NetworkClientDatabase {
		public:
			/**
			 * CTOR
			 * @param controller The networking controller
			 * @param logger What to use when logging information
			 */
			NetworkClientDatabase (
					network::winsock::NetworkController* controller
			);
		
		public:
			/**
			 * @inherit
			 */
			skillquest::network::Connection connect ( skillquest::network::Address address ) override;
			
		};
	}
}
