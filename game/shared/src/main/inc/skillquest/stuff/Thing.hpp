/**
 * @author omnisudo
 * @date 2024.03.12
 */

#pragma once

#include <typeindex>
#include <map>
#include <vector>
#include <memory>
#include "IThing.hpp"
#include "skillquest/stuff/Stuff.hpp"

namespace skillquest::stuff {
	class Thing : public virtual IThing {
	public:
		explicit Thing ( const IThing::CreateInfo& info );
		
		~Thing () override {
			return;
		}
		
		std::string toString () const override;
		
		auto uri () const -> URI final;
		
		auto stuff () -> IStuff& final;
		
		auto self () -> std::shared_ptr< IThing > final;
		
		auto active () -> bool final;
		
		auto activate () -> void final;
		
		auto deactivate () -> void final;
		
		auto onActivate () -> void override;
		
		auto onDeactivate () -> void override;
		
		auto parent () -> std::shared_ptr< IThing > final;
		
		auto root () -> std::shared_ptr< IThing > final;
		
		auto make_child_of ( std::weak_ptr< IThing > parent ) -> std::shared_ptr< IThing > final;
		
		auto children () -> std::map< URI, std::weak_ptr< IThing > > final;
		
		auto make_parent_of ( std::shared_ptr< IThing > child ) -> std::shared_ptr< IThing > final;
		
		auto connect (
				IComponent* component,
				const std::type_index& type
		) -> std::weak_ptr< IComponent > final;
		
		auto component ( const std::type_index& type ) -> std::weak_ptr< IComponent > final;
		
		auto with_component ( const std::type_index& type ) -> std::map< URI, std::weak_ptr< IComponent>> final;
		
		auto disconnect ( const std::type_index& type ) -> std::shared_ptr< IComponent > final;
	
	public:
		std::shared_ptr< IStuff > _stuff;
		
		URI _uri;
		
		bool _active = true;
		
		std::weak_ptr< IThing > _parent;
		
		std::map< URI, std::shared_ptr< IThing > > _children;
		
		std::map< std::type_index, std::shared_ptr< IComponent > > _components;
	};
}