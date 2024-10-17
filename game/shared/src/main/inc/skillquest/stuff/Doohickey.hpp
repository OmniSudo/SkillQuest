/**
 * @author omnisudo
 * @date 2024.03.25
 */

#pragma once

#include "skillquest/stuff.thing.hpp"

namespace skillquest::stuff {
class Doohickey : public Thing, public virtual IDoohickey {
	public:
        explicit Doohickey ( const IThing::CreateInfo& info ) : Thing{ info } {}
		
		~Doohickey() override = default;
	};
}