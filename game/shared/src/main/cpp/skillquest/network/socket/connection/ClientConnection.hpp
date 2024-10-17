#pragma once

#include "skillquest/network.hpp"
#include "skillquest/logger/ILogger.hpp"
#include <vector>

namespace skillquest::network::socket {
	class NetworkController;
	
	namespace database {
		class NetworkClientDatabase;
	}
	
	namespace connection {
		/**
		 * A credentials that is not the host; it is the client
		 * @author  OmniSudo
		 * @date    12/5/21
		 */
		class ClientConnection : public skillquest::network::connection::ClientConnection {
			/**
			 * socket
			 */
			int _socket;
			
			/**
			 * Data read from the socket
			 */
			std::vector< unsigned char > _buffer;
			
			std::mutex _receiving;
			
			std::mutex _sending;
			
			std::shared_future< std::shared_ptr< skillquest::network::IPacket > > _receiver;
		
		public:
			/**
			 * CTOR
			 * @param controller Network controller
			 * @param logger Logging object
			 * @param address Web address
			 */
			ClientConnection (
					network::socket::NetworkController* controller,
					skillquest::network::Address address
			);
			
			/**
			 * CTOR
			 * @param controller Network controller
			 * @param logger Logging object
			 * @param socket ASIO socket
			 */
			ClientConnection (
					network::socket::NetworkController* controller,
					skillquest::network::Address address,
					int socket
			);
			
			/**
			 * DTOR
			 */
			~ClientConnection () override;
		
		public:
			/**
			 * Connect to the servers @ the given address
			 */
			auto connect (
					const std::string& email, const std::string& password_hash, bool signup
			) -> void override;
			
			/**
			 * Check if the credentials exists between this and the servers
			 * @return True when exists
			 */
			bool connected () const override;
			
			/**
			 * Terminate the credentials
			 * @event network::event::DisconnectedEvent
			 */
			void disconnect () override;
		
		public:
			auto send ( json packet ) -> json override;
			
			auto receive () -> json override;
			
		};
	}
}
