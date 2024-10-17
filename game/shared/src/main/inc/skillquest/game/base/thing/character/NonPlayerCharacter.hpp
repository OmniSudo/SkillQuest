/**
 * @author omnisudo
 * @date 2024.07.26
 */

#pragma once

#include "Character.hpp"

namespace skillquest::game::base::thing::character {
	class NonPlayerCharacter : public Character {
	public:
		explicit NonPlayerCharacter ( const CreateInfo& info ) : Character{ info } {}
		
		~NonPlayerCharacter () override = default;
	};
}