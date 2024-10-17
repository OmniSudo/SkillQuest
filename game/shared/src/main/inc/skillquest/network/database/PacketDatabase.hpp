#pragma once

#include "skillquest/network/Packet.hpp"

#include "skillquest/network/connection/Connection.hpp"
#include "skillquest/logger.hpp"
#include <functional>
#include <mutex>
#include "skillquest/json.hpp"
#include <map>
#include <typeindex>
#include "skillquest/util/demangle.hpp"

namespace skillquest::network {
	class NetworkController;
	namespace database {
		/**
		 * Responsible for packets deserialization
		 * @author  OmniSudo
		 * @date    05.12.21
		 * @date    27.02.23
		 */
		class PacketDatabase {
			/**
			 * The networking renderer
			 */
			network::NetworkController* _controller;
			
			std::mutex _mutex;
		
		public:
			std::map<
					std::string /* TYPE */,
					std::function< network::Packet ( json ) > /* DESERIALIZER */
			> _packetDeserializers;
		
		public:
			explicit PacketDatabase (
					network::NetworkController* controller
			);
			
			virtual ~PacketDatabase () = default;
		
		public:
			/**
			 * Register a packets type
			 * @tparam TPacket The packets type
			 */
			template < typename TPacket >
			void add () {
				_packetDeserializers[ util::demangle( typeid( TPacket ) ) ] = [] (
						json data
				) -> std::shared_ptr< network::IPacket > {
					return std::make_shared< TPacket >( data );
				};
			}
			
			/**
			 * Unregister a packets type
			 * @tparam TPacket The packets type
			 */
			template < typename TPacket >
			void remove () {
				std::scoped_lock lock( _mutex );
				auto i = _packetDeserializers.find( util::demangle( typeid( TPacket ) ) );
				if ( i == _packetDeserializers.end() ) return;
				_packetDeserializers.erase( i );
			}
			
			/**
			 * Packets are formatted as:
			 * @param type The type of the session
			 * @param json Packet
			 * @return new session
			 */
			auto get ( std::type_index type, json json ) -> network::Packet;
			
			/**
			 * Packets are formatted as:
			 * @param type The type of the session ( string )
			 * @param json Packet
			 * @return new session
			 */
			auto get ( std::string type, json json ) -> network::Packet;
			
			/**
			 * Packets are formatted as:
			 * @param json Packet
			 * @return new session
			 */
			auto get ( json json ) -> network::Packet;
		};
	}
}