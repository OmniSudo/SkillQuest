#pragma once

#include <string>
#include <map>
#include <functional>
#include <future>
#include <typeindex>
#include <shared_mutex>
#include "skillquest/json.hpp"
#include "Address.hpp"
#include "skillquest/network/connection/Connection.hpp"
#include "skillquest/network/PacketListener.hpp"
#include "skillquest/util/demangle.hpp"

namespace skillquest::network {
	class NetworkController;
	
	/**
	 * @example Channel*& _channel = net.channels().creation( "name" )
	 * @author  OmniSudo
	 * @date    18.11.21
	 * @date    27.02.23
	 */
	class Channel {
	protected:
		/**
		 * The networking controller
		 */
		network::NetworkController* _controller;
		
		/**
		 * The name of the channels
		 */
		std::string _name;
		
		std::map< std::string, std::vector< std::shared_ptr< PacketListener > > > _callbacks;
		
		std::map< void* /* object */, std::map< std::string, std::shared_ptr< PacketListener > > > _instances;
		
		
		bool _encrypt;
		
		std::shared_mutex _mutex;
	
	public:
		Channel ( network::NetworkController* controller, std::string name, bool encrypt = false ) :
				_controller( controller ),
				_name( name ),
				_encrypt( encrypt ) {
		}
		
		virtual ~Channel ();
	
	public:
		inline network::NetworkController* controller () {
			return _controller;
		}
		
		inline std::string name () const {
			return _name;
		}
		
		inline bool encrypted () const {
			return _encrypt;
		}
	
	public:
		template < typename TPacket >
		auto send (
				network::Connection connection,
				TPacket* packet
		) -> network::Packet {
			return send( connection, network::Packet( static_cast< IPacket* >( packet ) ) );
		}
		
		template < typename TPacket >
		auto send (
				network::Connection connection,
				std::shared_ptr< TPacket > packet
		) -> std::shared_ptr< TPacket > {
			send(
					connection,
					std::static_pointer_cast< IPacket >( packet )
			);
			return packet;
		}
		
		auto send ( network::Connection connection, IPacket* packet ) -> Packet;
		
		auto send ( network::Connection connection, Packet packet ) -> Packet;
	
	private:
		auto send ( bool encrypted, network::Connection connection, Packet packet ) -> Packet;
	
	public:
		template < typename TPacket >
		auto add (
				std::function<
						void ( network::Connection connection, std::shared_ptr< TPacket > packet )
				> func
		) -> std::shared_ptr< PacketListener > {
			auto listener = std::make_shared< PacketListener >(
					typeid( TPacket ),
					[ func ] ( network::Connection connection, network::Packet packet ) {
						func( connection, std::static_pointer_cast< TPacket >( packet ) );
					},
					nullptr
			);
			
			auto demangled = util::demangle( typeid( TPacket ) );
			
			if ( !_callbacks.contains( demangled ) ) {
				_callbacks.emplace(
						demangled, std::vector< std::shared_ptr< PacketListener > >()
				);
			}
			
			_callbacks[ demangled ].emplace_back( listener );
			
			return listener;
		}
		
		template < typename TClass, typename TPacket >
		auto add (
				TClass* object,
				void( TClass::*method ) (
						network::Connection connection, std::shared_ptr< TPacket >
				)
		) -> std::shared_ptr< PacketListener > {
			auto listener = std::make_shared< PacketListener >(
					typeid( TPacket ),
					[ object, method ] ( network::Connection connection, Packet packet ) {
						( object->*method )( connection, std::static_pointer_cast< TPacket >( packet ) );
					},
					object
			);
			
			if ( !_callbacks.contains( listener->type ) ) {
				_callbacks.emplace( listener->type, std::vector< std::shared_ptr< PacketListener >>() );
			}
			
			if ( !_instances.contains( object ) ) {
				_instances.emplace( object, std::map< std::string, std::shared_ptr< PacketListener > >() );
			}
			_instances[ object ][ listener->type ] = listener;
			
			_callbacks[ listener->type ].emplace_back( listener );
			
			return listener;
		}
		
		template < typename TClass, typename TPacket >
		auto drop ( TClass* object,
					void( TClass::*method ) ( Connection, std::shared_ptr< TPacket > ) ) -> decltype( *this ) {
			if ( !_instances.contains( object ) ) return *this;
			auto objListeners = _instances[ object ];
			if ( !objListeners.contains( typeid( TPacket ) ) ) return *this;
			auto listener = objListeners[ typeid( TPacket ) ];
			if ( _callbacks.contains( listener->type ) ) {
				auto listeners = _callbacks[ listener->type ];
				listeners.erase(
						std::find_if(
								listeners.begin(), listeners.end(), [ listener ] ( const auto& other ) {
									return listener == other;
								}
						)
				);
				if ( listeners.empty() ) _callbacks.erase( listener->type );
			}
			objListeners.erase( typeid( TPacket ) );
			if ( objListeners.empty() ) _instances.erase( object );
			
			return *this;
		}
		
		template < typename TClass >
		auto drop ( TClass* object ) -> decltype( *this ) {
			if ( !_instances.contains( object ) ) return *this;
			auto objListeners = _instances.at( object );
			auto erase = std::vector< std::string >();
			for ( auto pair: objListeners ) {
				if ( _callbacks.contains( pair.first ) ) {
					auto listeners = _callbacks[ pair.second->type ];
					listeners.erase(
							std::find_if(
									listeners.begin(), listeners.end(), [ pair ] ( const auto& other ) {
										return pair.second == other;
									}
							)
					);
					if ( listeners.empty() ) _callbacks.erase( pair.second->type );
				}
				erase.push_back( pair.second->type );
			}
			
			for ( auto e: erase ) {
				objListeners.erase( e );
			}
			
			if ( objListeners.empty() ) _instances.erase( object );
			
			return *this;
		}
		
		auto drop ( std::shared_ptr< PacketListener > listener ) -> decltype( *this ) {
			if ( listener == nullptr ) return *this;
			
			auto demangled = util::demangle( listener->type );
			if ( !_callbacks.contains( demangled ) ) {
				return *this;
			}
			
			_callbacks[ demangled ].erase(
					std::remove_if(
							_callbacks[ demangled ].begin(), _callbacks[ demangled ].end(),
							[ listener ] ( const auto& other ) {
								return listener == other;
							}
					), _callbacks[ demangled ].end()
			);
			
			return *this;
		}
		
		template < typename TPacket >
		auto reset () -> decltype( *this ) {
			return reset( typeid( TPacket ) );
		}
		
		auto reset ( std::type_index type ) -> decltype( *this ) {
			auto demangled = util::demangle( type );
			if ( !_callbacks.contains( demangled ) ) return *this;
			auto listeners = _callbacks[ demangled ];
			for ( auto listener: listeners ) {
				if ( !_instances.contains( listener->subject ) ) continue;
				_instances[ listener->subject ].erase( listener->type );
				if ( _instances[ listener->subject ].empty() ) _instances.erase( listener->subject );
			}
			return *this;
		}
	
	private:
		void logError ( const std::string what );
	
	public:
		auto process ( Connection connection, Packet packet ) -> void;
		
		auto process ( Connection connection, Packet::element_type* packet ) -> void;
		
		auto process ( Connection connection, json json ) -> void;
	};
}

#define net_receive( name, packet ) void  onNet_##name ( skillquest::network::Connection connection, std::shared_ptr< packet > data )
