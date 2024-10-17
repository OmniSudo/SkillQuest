/**
 * @author omnisudo
 * @date 2024.08.25
 */

#pragma once

#include "skillquest/stuff.multiextendable.hpp"

namespace skillquest::game::base::thing::block {
    class IBlock : public virtual stuff::IMultiExtendable {

    };

    template< typename TBlock >
    class Block : public IBlock, public stuff::MultiExtendable< TBlock > {
    public:
        struct CreateInfo {
            const stuff::MultiExtendable< TBlock >::CreateInfo& extends;
        };

        Block ( const Block::CreateInfo& info ) : stuff::MultiExtendable< TBlock >{ info.extends } {}
    };
}