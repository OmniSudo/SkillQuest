/**
 * @author omnisudo
 * @date 2024.03.13
 */

#pragma once

#include "skillquest/stuff.thing.hpp"

namespace skillquest::stuff {
	template< typename TAttached >
	class Component : public virtual IComponent {
	public:
		~Component() override = default;
		
		auto thing() -> std::shared_ptr< IThing > override {
			return _thing.lock();
		}
		
		auto connect(
			std::shared_ptr< IThing > thing
		) -> std::weak_ptr< IComponent > override {
			_thing = thing;
			auto component = thing->component( typeid( TAttached ) );
			if ( auto c = component.lock(); !c || c->thing() != thing )
				component = thing->connect( this, typeid( TAttached ));
			onConnect( thing );
			return component;
		}
	
	protected:
		auto onConnect( std::shared_ptr< IThing > thing ) -> void override {}
	
	private:
		std::weak_ptr< IThing > _thing;
	};
}