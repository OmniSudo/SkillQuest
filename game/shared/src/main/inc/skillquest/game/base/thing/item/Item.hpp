/**
 * @author omnisudo
 * @date 2024.04.04
 */

#pragma once

#include "skillquest/stuff/MultiExtendable.hpp"

namespace skillquest::game::base::thing::item {
	class IItem : public virtual stuff::IMultiExtendable {
	};
	
	template < typename TItem >
	class Item : public IItem, public stuff::MultiExtendable< TItem > {
	public:
		struct CreateInfo {
			const typename stuff::MultiExtendable< TItem >::CreateInfo& extends;
		};
		
		Item ( const Item::CreateInfo& info ) : stuff::MultiExtendable< TItem >{ info.extends } {
		
		}
	};
}
