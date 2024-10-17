#pragma once

#include "IConnection.hpp"
#include "ClientConnection.hpp"
#include "skillquest/network/Address.hpp"
#include <memory>
#include <map>
#include "skillquest/crypto/cipher/RSA.hpp"

namespace skillquest::network {
	namespace connection {
		class ServerConnection;
	}
	
	typedef std::shared_ptr< connection::ServerConnection > Host;
	
	namespace connection {
		/**
		 *
		 * @author  OmniSudo
		 * @date    06.12.21
		 * @date    27.02.23
		 */
		class ServerConnection : public network::connection::IConnection {
			std::map< network::Address, std::shared_ptr< network::connection::ClientConnection > > _clients;
		
		property( rsa_key, crypto::cipher::Keypair, protected, private );
		
		public:
			/**
			 * CTOR
			 * @param addon
			 * @param address
			 */
			ServerConnection (
					network::NetworkController* controller,
					network::Address address
			) : IConnection( controller, address ) {
				_rsa_key = crypto::cipher::RSA::generate();
			}
			
			/**
			 * DTOR
			 */
			~ServerConnection () override;
		
		public:
			inline std::map< network::Address, std::shared_ptr< network::connection::ClientConnection > >&
			connections () {
				return _clients;
			}
			
			inline std::shared_ptr< network::connection::ClientConnection >
			connection ( const network::Address address ) {
				auto conns = connections();
				auto i = conns.find( address );
				if ( i == conns.end() ) return nullptr;
				return i->second;
			}
			
			/**
			 * Getter
			 * @return TRUE when a client is connected to the servers
			 */
			inline bool hosting () {
				return !connections().empty();
			}
			
			/**
			 * Disconnect from the servers
			 */
			virtual void disconnect () = 0;
			
		};
	}
}