/**
 * @author omnisudo
 * @data 2023.07.15
 */

#include "skillquest/event/EventBus.hpp"
#include "skillquest/sh.api.hpp"
#include <future>
#include <utility>

namespace skillquest::event {
	EventBus::EventBus () {
	
	}
	
	auto invoke ( std::function< void ( std::shared_ptr< IEvent > ) > func, std::shared_ptr< IEvent > event ) {
		func( std::move( event ) );
	}
	
	auto EventBus::post (
			std::shared_ptr< IEvent > event, std::vector< std::shared_ptr< EventListener > > listeners
	) -> void {
		std::vector< std::future< void > > f;
		for ( auto listener: listeners ) {
			try {
				if ( event->consumed() ) return;
				if ( !listener->filter( event ) ) continue;
				listener->callback( event );
			} catch ( std::exception& e ) {
				sq::shared()->logger()->error( "Failed to process event: {0}", e.what() );
			}
		}
	}
}
