/**
 * @author omnisudo
 * @date 2024.03.25
 */

#pragma once

#include "IThing.hpp"

namespace skillquest::stuff {
	/**
	 * A doohickey is the equivalent of a system in an ECS. A doohikey is stored in world and operates on other things
	 */
	class IDoohickey : public virtual IThing {
	public:
		~IDoohickey () override = default;
		
	};
}