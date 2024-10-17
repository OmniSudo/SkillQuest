/**
 * @author omnisudo
 * @data 2023.10.07
 */

#pragma once

#include <typeindex>
#include <functional>
#include <memory>

namespace skillquest::event {
	struct EventListener {
		void* subject;
		
		std::type_index type;
		
		std::function< void ( std::shared_ptr< IEvent > ) > callback;
		
		std::function< bool ( std::shared_ptr< IEvent > ) > filter = [] ( auto event ) { return true; };
	
	public:
		EventListener (
				std::type_index type,
				std::function< void ( std::shared_ptr< IEvent > ) > callback,
				void* subject = nullptr
		) :
				subject( subject ),
				type( type ),
				callback( callback ) {
		}
	};
}
