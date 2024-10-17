/**
 * @author omnisudo
 * @date 2024.03.11
 */

#pragma once

#include <memory>
#include <map>
#include <deque>
#include <typeindex>
#include "skillquest/string.hpp"
#include "skillquest/uri.hpp"
#include "IComponent.hpp"
#include "skillquest/property.hpp"

namespace skillquest::stuff {
	class IStuff;
	
	class IThing : public virtual util::ToString, public virtual HasURI {
	public:
        friend class Stuff;

		struct CreateInfo {
			URI uri;
			std::shared_ptr< IStuff > stuff = nullptr;
		};
	
	public:
		/**
		 * DTOR
		 */
		~IThing() override = default;

		/**
		 * @return stuff that contains this thing
		 */
		virtual auto stuff () -> IStuff& = 0;
		
		virtual auto self () -> std::shared_ptr< IThing > = 0;
		
		virtual auto onActivate () -> void = 0;
		
		virtual auto onDeactivate () -> void = 0;
		
		/**
		 * Is this thing "on"
		 * @return True when on
		 */
		virtual auto active () -> bool = 0;
		
		/**
		 * Turn on
		 */
		virtual auto activate () -> void = 0;
		
		/**
		 * Turn off
		 */
		virtual auto deactivate () -> void = 0;
		
		auto active ( bool value ) -> void {
			value ? activate() : deactivate();
		}
		
		/**
		 * If this thing is a child of another thing return the parent
		 * @return parent
		 */
		virtual auto parent () -> std::shared_ptr< IThing > = 0;

		/**
		 * Get the top level thing this is a child of
		 * @return parent / this if no parent
		 */
		virtual auto root () -> std::shared_ptr< IThing > = 0;
		
		/**
		 * Make this thing a child of parent
		 * @param parent The parent
		 * @return Itself
		 */
		virtual auto make_child_of ( std::weak_ptr< IThing > parent ) -> std::shared_ptr< IThing > = 0;

		/**
		 * Get children
		 * @return all children
		 */
		virtual auto children () -> std::map< URI, std::weak_ptr< IThing > > = 0;

		/**
		 * Make this thing the parent of the child
		 * @param child The child
		 * @return Itself
		 */
		virtual auto make_parent_of ( std::shared_ptr< IThing > child ) -> std::shared_ptr< IThing > = 0;
		
		/**
		 * Attach a component of type TComponent identified by itself
		 * @tparam TComponent Component and Identifier type
		 * @param component Component to attach to this thing
		 * @return Attached component
		 */
		template< typename TComponent >
		auto connect( TComponent *component ) -> std::weak_ptr< TComponent > {
			return connect< TComponent, TComponent >( component );
		}
		
		/**
		 * Attach a component of type TComponent identified by TAttached
		 * @tparam TComponent Component type
		 * @tparam TAttached Identifier type
		 * @param component The component to attach to this thing
		 * @return Attached component
		 */
		template<
				typename TComponent,
				typename TAttached,
				typename = std::enable_if< std::is_base_of_v< IComponent, TComponent > >
		>
		auto connect( TComponent *component ) -> std::weak_ptr< TComponent > {
			return std::reinterpret_pointer_cast< TComponent >( connect( component, typeid( TAttached )).lock() );
		}
		
		/**
		 * Attach a component identified by passed type
		 * @param component Component to attach to this thing
		 * @param type Component identifier
		 * @return Attached component
		 */
		virtual auto connect(
				IComponent *component,
				const std::type_index &type
		) -> std::weak_ptr< IComponent > = 0;
	
	public:
		/**
		 * Get a component of type TComponent identified by itself
		 * @tparam TComponent Component and Identifier type
		 * @return Component
		 */
		template< typename TComponent >
		auto component() -> std::weak_ptr< TComponent > {
			return component< TComponent, TComponent >();
		}
		
		/**
		 * Get a component of type TComponent identified by TAttached
		 * @tparam TComponent Component type
		 * @tparam TAttached Identifier type
		 * @return Component
		 */
		template<
				typename TComponent,
				typename TAttached,
				typename = std::enable_if< std::is_base_of_v< IComponent, TComponent > >
		>
		auto component() -> std::weak_ptr< TComponent > {
			return std::dynamic_pointer_cast< TComponent >( component( typeid( TAttached )).lock());
		}
		
		/**
		 * Get a component of passed identifier type
		 * @param type Component identifier
		 * @return Component
		 */
		virtual auto component( const std::type_index &type ) -> std::weak_ptr< IComponent > = 0;
		
		/**
		 * Get all components of type TComponent identified by itself in all children
		 * @tparam TComponent Component type
		 * @return URI -> Component
		 */
		template< typename TComponent >
		auto with_component() -> std::map< URI, std::weak_ptr< TComponent > > {
			return with_component< TComponent, TComponent >();
		}
		
		/**
		 * Get all components of type TComponent identified by TAttached in all children
		 * @tparam TComponent Component type
		 * @tparam TAttached Identifier type
		 * @return URI -> Component
		 */
		template<
				typename TComponent,
				typename TAttached,
				typename = std::enable_if< std::is_base_of_v< IComponent, TComponent > >
		>
		auto with_component() -> std::map< URI, std::weak_ptr< TComponent > > {
			auto comps = with_component( typeid( TAttached ));
			std::map< URI, std::weak_ptr< TComponent > > casted;
			for ( auto& pair : comps ) {
				casted[ pair.first ] = std::dynamic_pointer_cast< TComponent >( pair.second.lock() );
			}
			return casted;
		}
		
		/**
		 * Get components of type from all children
		 * @param type Type of the component identifier
		 * @return URI -> Component
		 */
		virtual auto with_component( const std::type_index &type ) -> std::map< URI, std::weak_ptr< IComponent > > = 0;
		
		template < typename TThing, typename = std::enable_if_t< std::is_base_of_v< IThing, TThing > > >
		auto with_thing() -> std::map< URI, std::weak_ptr< TThing > > {
			std::deque< IThing* > things = { this };
			std::map< URI, std::weak_ptr< TThing > > ret;
			while ( !things.empty() ) {
				auto front = things.front();
				things.pop_front();
				
				if ( !front ) continue;
				
				for (auto &child: front->children() ) {
					things.push_back( child.second.lock().get());
				}
				
				auto thing = dynamic_cast< TThing* >( front );
				if ( !thing ) continue;
				ret[ thing->uri() ] = std::dynamic_pointer_cast< TThing >( stuff()[ thing->uri() ] );
			}
			
			return ret;
		};
		
		/**
		 * Remove a component
		 * @tparam TComponent Type of the component ( & identifier )
		 * @return Removed component
		 */
		template< typename TComponent >
		auto disconnect() -> std::shared_ptr< TComponent > {
			return disconnect< TComponent, TComponent >();
		}
		
		/**
		 * Remove a component
		 * @tparam TComponent The type of the component
		 * @tparam TAttached The type of the component identifier
		 * @return Removed component
		 */
		template<
				typename TComponent,
				typename TAttached,
				typename = std::enable_if< std::is_base_of_v< IComponent, TComponent > >
		>
		auto disconnect() -> std::shared_ptr< TComponent > {
			return std::dynamic_pointer_cast< TComponent >( disconnect( typeid( TAttached )));
		}
		
		/**
		 * Remove a component
		 * @param type The component identifier
		 * @return Removed component
		 */
		virtual auto disconnect( const std::type_index &type ) -> std::shared_ptr< IComponent > = 0;

    private:
        property( ptr_to_self, std::weak_ptr< IThing >, protected, protected );

	};
}