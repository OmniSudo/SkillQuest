/**
 * @author omnisudo
 * @date 2024.07.30
 */

#pragma once

#include "skillquest/game/base/thing/item/Item.hpp"
#include "skillquest/game/base/thing/item/ItemStack.hpp"

namespace sq::sh {
	typedef std::shared_ptr< skillquest::game::base::thing::item::IItem > IItem;

	template < typename TItem >
	using Item = std::shared_ptr< skillquest::game::base::thing::item::Item< TItem > >;

	typedef std::shared_ptr< skillquest::game::base::thing::item::ItemStack > ItemStack;
}