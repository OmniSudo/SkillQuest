/**
 * @author  OmniSudo
 * @date    2024.04.02
 */

#pragma once

#include "skillquest/uid.hpp"
#include "skillquest/random/String.hpp"
#include "skillquest/stuff/MultiExtendable.hpp"
#include "Item.hpp"
#include "skillquest/stuff.thing.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::thing::item {
	class ItemStack : public stuff::Thing {
	public:
		struct CreateInfo {
			util::UID id = {};
            std::shared_ptr< character::Character > owner;
			std::shared_ptr< stuff::IThing > item;
			std::size_t count;
		};
	
	private:
        property( owner, std::shared_ptr< character::Character >, public, public );
        property( item, std::shared_ptr< stuff::IThing >, none, none );
		property( count, std::size_t, public_ref, public );
	
	public:
		ItemStack ( const CreateInfo& info ) :
				stuff::Thing{
						{
								.uri = { "itemstack://skill.quest/" + info.id.toString() }
						}
				},
				_item{ info.item->root() },
				_count{ info.count },
                _owner{ info.owner } {
		}
		
		auto item () -> std::shared_ptr< IThing > {
			return _item->root();
		}

        auto item ( std::shared_ptr< stuff::IThing > value ) -> decltype(*this)& {
            _item = value->root();
            return *this;
        }
	};
}