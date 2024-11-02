/**
 * @author omnisudo
 * @date 2024.03.11
 */

#pragma once

#include <vector>
#include <memory>
#include "IThing.hpp"

namespace skillquest::stuff {
	class IStuff {
	public:
		struct CreateInfo {
		
		};
		
		IStuff ( const CreateInfo& info ) {}
		
		virtual ~IStuff () = default;
	
	public:
		/**
		 * Create a thing of type TThing
		 * @tparam TThing The thing type
		 * @param info The thing's CreateInfo
		 * @return The thing as a shared ptr
		 */
		template < class TThing, typename = std::enable_if< std::is_base_of_v< IThing, TThing > > >
		auto create ( const typename TThing::CreateInfo& info, bool activate = true ) -> std::shared_ptr< TThing > {
			auto thing = std::make_shared< TThing >( info );
			auto tthing = std::dynamic_pointer_cast< TThing >( add( thing ) ); // was add( thing ) but thing adds itself to us
            if ( activate ) tthing->activate();
            return tthing;
		}
		
		template < class TThing, typename = std::enable_if< std::is_base_of_v< IThing, TThing > > >
		auto create ( bool activate = true ) {
			auto thing = std::make_shared< TThing >();
			auto tthing = std::dynamic_pointer_cast< TThing >( add( thing ) );
            if ( activate ) tthing->activate();
            return tthing;
		}
		
		/**
		 * Check if thing with uri exists
		 * @param uri Uri of the thing
		 * @return True when found
		 */
		virtual auto contains ( const URI& uri ) -> bool = 0;
		
		/**
		 * Get a thing
		 * @param uri The things uriT
		 * @return The thing
		 */
		virtual auto operator[] ( const URI& uri ) -> std::shared_ptr< IThing >& = 0;
		
		template < typename TThing, typename = std::enable_if_t< std::is_base_of_v< IThing, TThing > > >
		auto at ( const URI& uri ) -> std::map< URI, std::weak_ptr< TThing > > {
			return operator[]( uri )->with_thing< TThing >();
		}
		
		template < typename TThing, typename = std::enable_if_t< std::is_base_of_v< IThing, TThing > > >
		auto first ( const URI& uri ) -> std::shared_ptr< TThing > {
			auto things = at< TThing >( uri );
			auto thing = things.begin();
			
			if ( thing == things.end() ) return nullptr;
			
			return thing->second.lock();
		}
		
		/**
		 * Get all things
		 * @return
		 */
		virtual auto things () -> std::vector< std::weak_ptr< IThing > > = 0;
		
		/**
		 * Get all things that are sub-URIs of search_partial
		 * @param search_partial The parent uri
		 * @return Things that were found
		 */
		virtual auto index ( const URI& search_partial ) -> std::map< URI, std::weak_ptr< IThing > > = 0;
		
		/**
		 * Get all things that have URIs of a specific scheme
		 * @param scheme The scheme "scheme://uri"
		 * @return Things
		 */
		virtual auto scheme ( const std::string& scheme ) -> std::map< URI, std::weak_ptr< IThing > > = 0;
		
		/**
		 * Remove a thing from the stuff
		 * @param uri The uri of the thing
		 * @return true if removed
		 */
		virtual auto remove ( const URI& uri ) -> bool = 0;
		
		/**
		 * Remove a thing from the stuff
		 * @param thing Can be nullptr
		 * @return true if removed
		 */
		virtual auto remove ( std::shared_ptr< IThing > thing ) -> bool = 0;
		
		/**
		 * Get all components of type TComponent identified by itself in all children
		 * @tparam TComponent Component type
		 * @return URI -> Component
		 */
		template < typename TComponent >
		auto with () -> std::map< URI, std::weak_ptr< TComponent > > {
			return with< TComponent, TComponent >();
		}
		
		/**
		 * Get all components of type TComponent identified by TAttached in all children
		 * @tparam TComponent Component type
		 * @tparam TAttached Identifier type
		 * @return URI -> Component
		 */
		template <
				typename TComponent,
				typename TAttached,
				typename = std::enable_if< std::is_base_of_v< IComponent, TComponent > >
		>
		auto with () -> std::map< URI, std::weak_ptr< TComponent > > {
			auto comps = with( typeid( TAttached ) );
			std::map< URI, std::weak_ptr< TComponent > > casted;
			for ( auto& pair: comps ) {
				casted[ pair.first ] = std::dynamic_pointer_cast< TComponent >( pair.second.lock() );
			}
			return casted;
		}
		
		/**
		 * Get components of type from all children
		 * @param type Type of the component identifier
		 * @return URI -> Component
		 */
		virtual auto with ( const std::type_index& type ) -> std::map< URI, std::weak_ptr< IComponent > > = 0;
	
	public:
		/**
		 * Add a thing to the list of tracked things
		 * @param thing The thing
		 * @return The thing as a shared ptr
		 */
		virtual auto add ( IThing* thing ) -> std::shared_ptr< IThing > = 0;
		
		virtual auto add ( std::shared_ptr< IThing > thing ) -> std::shared_ptr< IThing > = 0;

	};
}
