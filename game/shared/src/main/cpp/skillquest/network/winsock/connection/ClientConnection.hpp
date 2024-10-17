#pragma once

#include "skillquest/network.hpp"
#include "skillquest/logger/ILogger.hpp"
#include <vector>

namespace skillquest::network::winsock {
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
			void* _socket;
			
			/**
			 * Data read from the socket
			 */
			std::vector< unsigned char > _buffer;
			
			std::mutex _receiving;
			
			std::mutex _sending;
			
			std::shared_future< std::shared_ptr< skillquest::network::IPacket > > _receiver;
			
			std::future< void > _process;
		
		public:
			/**
			 * CTOR
			 * @param controller Network controller
			 * @param logger Logging object
			 * @param address Web address
			 */
			ClientConnection (
					network::winsock::NetworkController* controller,
					skillquest::network::Address address
			);
			
			/**
			 * CTOR
			 * @param controller Network controller
			 * @param logger Logging object
			 * @param socket ASIO socket
			 */
			ClientConnection (
					network::winsock::NetworkController* controller,
					network::Address address,
					void* socket
			);
			
			/**
			 * DTOR
			 */
			~ClientConnection () override;
		
		public:
			/**
			 * Connect to the servers @ the given address
			 */
			auto connect () -> void override;
			
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
