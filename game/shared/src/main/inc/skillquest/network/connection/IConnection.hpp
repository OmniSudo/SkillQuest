#pragma once

#include "skillquest/network/Address.hpp"
#include "skillquest/logger.hpp"
#include <memory>
#include <string>
#include <map>
#include "skillquest/property.hpp"


namespace skillquest::network {
	class NetworkController;
	
	namespace connection {
		/**
		  * A Generic net credentials
		  * @author  OmniSudo
		  * @date    18.11.21
		  * @date    27.02.23
		  */
		class IConnection {
		protected:
			network::NetworkController* _controller;
			
			network::Address _address;
		
		public:
			IConnection (
					network::NetworkController* controller,
					network::Address address
			);
			
			virtual ~IConnection () = default;
		
		public:
			inline network::NetworkController* controller () {
				return _controller;
			}
			
			inline network::Address address () {
				return _address;
			}
		};
	}
}
