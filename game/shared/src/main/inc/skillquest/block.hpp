/**
 * @author omnisudo
 * @date 2024.08.25
 */

#pragma once

#include "skillquest/game/base/thing/block/Block.hpp"

namespace sq::sh {
    typedef std::shared_ptr< skillquest::game::base::thing::block::IBlock > IBlock;

    template< typename TBlock >
    using Block = std::shared_ptr< skillquest::game::base::thing::block::Block< TBlock > >;
}