#pragma once

#include <memory>
#include "skillquest/network/Address.hpp"
#include "skillquest/network/connection/IConnection.hpp"
#include "skillquest/network/connection/ClientConnection.hpp"
#include <map>

namespace skillquest::network {
	class NetworkController;
	
	namespace database {
		/**
		 * Tracks client connections
		 * @author  OmniSudo
		 * @date    05.12.21
	     * @date    27.02.23
		 */
		class NetworkClientDatabase {
		protected:
			friend class network::connection::ClientConnection;
			
			/**
			* The networking controller
			*/
			network::NetworkController* _controller;
			
			/**
			* All connected clients
			*/
			std::map< network::Address, std::shared_ptr< network::connection::ClientConnection > > _clients;
		
		public:
			/**
			* CTOR
			* @param mod Networking renderer
			*/
			NetworkClientDatabase (
					network::NetworkController* controller
			);
			
			/**
			* DTOR
			*/
			virtual ~NetworkClientDatabase () = default;
		
		public:
			/**
			* GETTER
			* @return Networking Module
			*/
			inline network::NetworkController* controller () {
				return _controller;
			}
			
			/**
			* GETTER
			* @return Connected clients
			*/
			inline std::map< network::Address, std::shared_ptr< network::connection::ClientConnection > >&
			connections () {
				return _clients;
			}
			
			/**
			* SETTER
			* @param connection Client credentials
			* @return The stored credentials
			*/
			std::shared_ptr< network::connection::ClientConnection >
			connection ( std::shared_ptr< network::connection::ClientConnection > connection );
			
			/**
			* GETTER
			* @param address HostConnection address
			* @return The credentials, nullptr if not connected
			*/
			std::shared_ptr< network::connection::ClientConnection > connection ( network::Address address );
		
		public:
			/**
			* Connect to a servers
			* @param address The address of the servers
			* @return A servers credentials, can be disconnected
			*/
			virtual std::shared_ptr< network::connection::ClientConnection > connect (
					network::Address address,
					const std::string& email,
					const std::string& password_hash,
					bool signup
			) = 0;
			
		};
	}
}
