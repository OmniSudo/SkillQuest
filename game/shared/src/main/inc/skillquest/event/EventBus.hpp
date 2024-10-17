/**
 * @author omnisudo
 * @data 2023.07.15
 */

#pragma once

#include <memory>
#include <map>
#include <typeindex>
#include <vector>
#include <functional>
#include "IEvent.hpp"
#include "EventListener.hpp"

namespace skillquest::event {
	class EventBus {
	private:
		std::map< std::type_index, std::vector< std::shared_ptr< EventListener > > > _callbacks;
		
		std::map< void* /* object */, std::map< std::type_index, std::shared_ptr< EventListener > > > _instances;
	
	public:
		explicit EventBus ();
	
	public:
		template < typename TEvent >
		auto add ( std::function< void ( std::shared_ptr< TEvent > ) > func ) -> std::shared_ptr< EventListener > {
			auto listener = std::make_shared< EventListener >(
					typeid( TEvent ),
					[ func ] ( std::shared_ptr< IEvent > event ) {
						func( std::static_pointer_cast< TEvent >( event ) );
					},
					nullptr
			);
			
			if ( !_callbacks.contains( typeid( TEvent ) ) ) {
				_callbacks.emplace(
						typeid( TEvent ), std::vector< std::shared_ptr< EventListener > >()
				);
			}
			
			_callbacks[ typeid( TEvent ) ].emplace_back( listener );
			
			return listener;
		}
		
		template < typename TClass, typename TEvent >
		auto add ( TClass* object, void( TClass::*method ) ( std::shared_ptr< TEvent > ) ) -> std::shared_ptr<
				EventListener
		> {
			auto listener = std::make_shared< EventListener >(
					typeid( TEvent ),
					[ object, method ] ( std::shared_ptr< IEvent > event ) {
						( object->*method )( std::static_pointer_cast< TEvent >( event ) );
					},
					object
			);
			
			if ( !_callbacks.contains( listener->type ) ) {
				_callbacks.emplace( listener->type, std::vector< std::shared_ptr< EventListener >>() );
			}
			
			if ( !_instances.contains( object ) ) {
				_instances.emplace( object, std::map< std::type_index, std::shared_ptr< EventListener > >() );
			}
			_instances[ object ][ listener->type ] = listener;
			
			_callbacks[ listener->type ].emplace_back( listener );
			
			return listener;
		}
		
		template < typename TClass, typename TEvent >
		auto drop ( TClass* object, void( TClass::*method ) ( std::shared_ptr< TEvent > ) ) -> decltype( *this ) {
			if ( !_instances.contains( object ) ) return *this;
			auto objListeners = _instances[ object ];
			if ( !objListeners.contains( typeid( TEvent ) ) ) return *this;
			auto listener = objListeners[ typeid( TEvent ) ];
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
			objListeners.erase( typeid( TEvent ) );
			if ( objListeners.empty() ) _instances.erase( object );
			
			return *this;
		}
		
		template < typename TClass >
		auto drop ( TClass* object ) -> decltype( *this ) {
			if ( !_instances.contains( object ) ) return *this;
			auto objListeners = _instances.at( object );
			auto erase = std::vector< std::type_index >();
			for ( auto pair: objListeners ) {
				if ( _callbacks.contains( pair.first ) ) {
					auto& listeners = _callbacks[ pair.second->type ];
					listeners.erase(
							std::find_if(
									listeners.begin(), listeners.end(), [ pair ] ( const auto& other ) {
										return pair.second.get() == other.get();
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
		
		auto remove ( std::shared_ptr< EventListener > listener ) -> decltype( *this ) {
			if ( listener == nullptr ) return *this;
			
			if ( !_callbacks.contains( listener->type ) ) {
				return *this;
			}
			
			_callbacks[ listener->type ].erase(
					std::remove_if(
							_callbacks[ listener->type ].begin(), _callbacks[ listener->type ].end(),
							[ listener ] ( const auto& other ) {
								return listener.get() == other.get();
							}
					), _callbacks[ listener->type ].end()
			);
			
			return *this;
		}
		
		template < typename TEvent >
		auto reset () -> decltype( *this ) {
			return reset( typeid( TEvent ) );
		}
		
		auto reset ( std::type_index type ) -> decltype( *this ) {
			if ( !_callbacks.contains( type ) ) return *this;
			auto listeners = _callbacks[ type ];
			for ( auto listener: listeners ) {
				if ( !_instances.contains( listener->subject ) ) continue;
				_instances[ listener->subject ].erase( listener->type );
				if ( _instances[ listener->subject ].empty() ) _instances.erase( listener->subject );
			}
			return *this;
		}
		
		template < typename TEvent >
		auto post ( TEvent* event ) -> EventBus& {
			return post( std::shared_ptr< TEvent >( event ) );
		}
		
		template < typename TEvent >
		auto post ( std::shared_ptr< TEvent > event ) -> EventBus& {
			if ( _callbacks.contains( typeid( TEvent ) ) ) {
				post( std::dynamic_pointer_cast< IEvent >( event ), _callbacks[ typeid( TEvent ) ] );
			}
			return *this;
		}
		
		template < typename TEvent >
		auto post ( std::shared_ptr< IEvent > event, std::type_index type ) -> EventBus& {
			if ( _callbacks.contains( type ) ) {
				post( std::static_pointer_cast< IEvent >( event ), _callbacks[ type ] );
			}
			return *this;
		}
	
	private:
		auto post (
				std::shared_ptr< IEvent > event,
				std::vector< std::shared_ptr< EventListener > > listeners
		) -> void;
	};
}
