/**
 * @author omnisudo
 * @date 2024.07.30
 */

#pragma once

#include "skillquest/game/base/thing/item/Item.hpp"
#include "skillquest/game/base/thing/item/ItemStack.hpp"

namespace sq::sh {
	typedef std::shared_ptr< skillquest::game::base::thing::item::IItem > IItem;
	typedef IItem::element_type IITEM;

	template < typename TItem >
	using Item = std::shared_ptr< skillquest::game::base::thing::item::Item< TItem > >;

	template < typename TItem >
	using ITEM = typename Item< TItem >::element_type;

	typedef std::shared_ptr< skillquest::game::base::thing::item::ItemStack > ItemStack;
	typedef ItemStack::element_type ITEMSTACK;
}