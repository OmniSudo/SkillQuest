#pragma once

#include <map>
#include <string>
#include <memory>
#include "skillquest/network/Channel.hpp"
#include "skillquest/string.hpp"

namespace skillquest::network {
	class NetworkController;
	
	namespace database {
		/**
		 * Tracks net channels
		 * @author  OmniSudo
		 * @date    05.12.21
	     * @date    27.02.23
		 */
		class ChannelDatabase {
		private:
			friend class network::Channel;
			
			friend class skillquest::network::NetworkController;
			
			/**
			* The net renderer
			*/
			network::NetworkController* _controller;
		
		public:
			/**
			* IPacket channels
			*/
			std::map< std::string /* NAME */, network::Channel* /* CHANNEL */ > _channels;
		
		public:
			/**
			* CTOR
			* @param mod Network renderer
			*/
			explicit ChannelDatabase ( network::NetworkController* controller );
			
			/**
			* DTOR
			*/
			virtual ~ChannelDatabase () = default;
		
		public:
			/**
			* Helper method to cast a channels name for a specific instance of a class
			* @tparam TClass The class type
			* @param object The class object instance
			* @return The created channels
			*/
			template < class TClass >
			inline std::string name ( const TClass& object ) const {
				auto type = std::string( typeid( TClass ).name() );
				auto ptr = util::toString( reinterpret_cast< std::size_t >( *object ) );
				return type + ":" + ptr;
			}
			
			/**
			* Helper method to cast a channels name for a shared ptr
			* @tparam TClass The class type
			* @param object The class object instance
			* @return The created channels
			*/
			template < class TClass >
			inline std::string name ( const TClass* object ) {
				return name( &*object );
			}
			
			/**
			* Create a credentials
			* @param id The name of the channels
			* @return The created channels
			*/
			Channel*& create ( std::string id, bool encrypt = true );
			
			/**
			* Get a created channels
			* @param id The name of the channels
			* @return The ( already ) created channels
			*/
			Channel*& get ( std::string id );
			
			void destroy ( Channel* channel );
		
		private:
			void onConnected ( network::Connection connection );
			
		};
	}
}