#pragma once

#include <memory>
#include "skillquest/network/Address.hpp"
#include "skillquest/network/connection/IConnection.hpp"
#include "skillquest/network/connection/ServerConnection.hpp"
#include "skillquest/network/controller/handshake/IAuthenticator.hpp"

namespace skillquest::network {
	class NetworkController;
	
	namespace database {
		/**
		 *
		 * @author  OmniSudo
		 * @date    05.12.21
		 * @date    27.02.23
		 */
		class NetworkServerDatabase {
		private:
			friend class network::connection::ServerConnection;
		
		protected:
			/**
			* The networking controller
			*/
			network::NetworkController* _controller;
			
			/**
			* All connected clients
			*/
			std::map< network::Address, std::shared_ptr< network::connection::ServerConnection > > _servers;
		
		public:
			explicit NetworkServerDatabase (
					network::NetworkController* controller
			);
			
			virtual ~NetworkServerDatabase () = default;
		
		public:
			inline network::NetworkController* controller () {
				return _controller;
			}
			
			inline std::map< network::Address, std::shared_ptr< network::connection::ServerConnection > >& hosts () {
				return _servers;
			}
		
		property( authenticator, std::shared_ptr< controller::handshake::IAuthenticator >, public, public )
			
			/**
			 * This is for creating servers, see host for getting them
			 * @param address
			 * @return
			 */
			virtual auto open (
					const network::Address& address
			) -> std::shared_ptr< network::connection::ServerConnection > = 0;
			
			/**
			 * This is for getting existing servers, see open for creating a server
			 * @param address
			 * @return
			 */
			inline std::shared_ptr< network::connection::ServerConnection > host ( const network::Address& address ) {
				auto i = hosts().find( address );
				if ( i == hosts().end() ) return nullptr;
				return i->second;
			}
		};
	}
}
