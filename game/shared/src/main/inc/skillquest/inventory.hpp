/**
 * @author  omnisudo
 * @date    2024.11.01
 */

#pragma once

#include <skillquest/game/base/thing/inventory/Inventory.hpp>

namespace sq::sh {
    typedef std::shared_ptr< skillquest::game::base::thing::inventory::Inventory > Inventory;
    typedef Inventory::element_type INVENTORY;
}