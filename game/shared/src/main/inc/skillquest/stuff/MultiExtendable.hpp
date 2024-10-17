/**
 * @author  OmniSudo
 * @date    2024.04.03
 */

#pragma once

#include "Thing.hpp"
#include <regex>
#include "skillquest/property.hpp"

namespace skillquest::stuff {
	class IMultiExtendable : public virtual IThing {
	public:
		struct CreateInfo {
			URI base = { "" };
			URI uri = { "" };
		};
	
	public:
		template <
				typename TComponent,
				typename = std::enable_if_t< std::is_base_of_v< IComponent, TComponent > >
		>
		auto of () -> std::shared_ptr< TComponent > {
			auto root = this->root();
			auto comps = root->with_component< TComponent >();
			auto comp = comps.begin();
			if ( comp == comps.end() ) return nullptr;
			return comp->second.lock();
		}
		
		template <
				typename TThing,
				typename = std::enable_if_t< std::is_base_of_v< IThing, TThing > >
		>
		auto being () -> std::shared_ptr< TThing > {
			auto root = this->root();
			auto things = root->with_thing< TThing >();
			auto thing = things.begin();
			if ( thing == things.end() ) return nullptr;
			return thing->second.lock();
		}
	};
	
	template < typename TThing >
	class MultiExtendable : public IMultiExtendable, public stuff::Thing {
	public:
		MultiExtendable ( const IMultiExtendable::CreateInfo& info ) :
				stuff::Thing{
						{
								.uri =  info.uri
						}
				} {
			if ( !info.base.toString().empty() && info.uri.toString() != info.base.toString() ) {
				if ( !stuff().contains( info.base ) ) {
					stuff::Thing::stuff().template create< stuff::Thing >(
							{
									.uri = info.base
							}
					);
				}
				_base = stuff()[ info.base ]->root();
			}
		}

        auto onActivate () -> void override {
            Thing::onActivate();

            if ( _base ) _base->make_parent_of( stuff::Thing::self() );
        }

        property( base, std::shared_ptr< stuff::IThing >, public, none );
    };
	
}
