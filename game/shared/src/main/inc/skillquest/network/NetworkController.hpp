#pragma once

#include "skillquest/network/database/PacketDatabase.hpp"
#include "skillquest/network/database/NetworkClientDatabase.hpp"
#include "skillquest/network/database/NetworkServerDatabase.hpp"
#include "skillquest/network/database/ChannelDatabase.hpp"
#include "skillquest/logger.hpp"
#include "skillquest/network/packet/handshake/ConnectedPacket.hpp"
#include "skillquest/network/packet/handshake/PublicKeyPacket.hpp"
#include "skillquest/network/packet/handshake/EncryptedCredentialsPacket.hpp"
#include "skillquest/network/packet/handshake/EncryptedSignupCredentialsPacket.hpp"
#include "skillquest/network/packet/handshake/AuthenticationStatusPacket.hpp"

namespace skillquest {
	namespace engine {
		class IEngine;
	}
	
	namespace network {
		/**
		*
		* @author  OmniSudo
		* @date    12.22.21
		* @date    27.02.23
		*/
		class NetworkController {
		private:
			friend class database::ChannelDatabase;
			
			friend class connection::ClientConnection;
			
			friend class connection::ServerConnection;
			
			friend class Channel;
		
		protected:
			network::database::PacketDatabase* _packets = nullptr;
			
			network::database::ChannelDatabase* _channels = nullptr;
			
			network::database::NetworkClientDatabase* _clients = nullptr;
			
			network::database::NetworkServerDatabase* _servers = nullptr;
			
			network::Channel* _channel = nullptr;
		
		public:
			explicit NetworkController ();
			
			virtual ~NetworkController ();
		
		public:
			/**
			* GETTER
			* Registers packets, using this->packets()->add< TPacket >();
			* @return The packets database, used to deserialize packets
			*/
			inline network::database::PacketDatabase& packets () {
				return *_packets;
			}
			
			/**
			* GETTER
			* @return Creates and keeps track of channels
			*/
			inline network::database::ChannelDatabase& channels () {
				return *_channels;
			}
			
			/**
			* GETTER
			* @return Handles client connections
			*/
			inline network::database::NetworkClientDatabase& clients () {
				return *_clients;
			}
			
			/**
			* GETTER
			* @return Handles servers hosting
			*/
			inline network::database::NetworkServerDatabase& servers () {
				return *_servers;
			}
		
		protected:
			net_receive( PublicKeyPacket, packet::handshake::PublicKeyPacket );
			
			net_receive( EncryptedCredentialsPacket, packet::handshake::EncryptedCredentialsPacket );
			
			net_receive( EncryptedSignupCredentialsPacket, packet::handshake::EncryptedSignupCredentialsPacket );
			
			net_receive( AuthenticationStatusPacket, packet::handshake::AuthenticationStatusPacket );
			
		};
	}
}