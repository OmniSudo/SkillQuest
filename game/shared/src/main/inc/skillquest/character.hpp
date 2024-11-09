/**
 * @author omnisudo
 * @date 2024.07.26
 */

#pragma once

#include "skillquest/game/base/thing/character/Character.hpp"
#include "skillquest/game/base/thing/character/PlayerCharacter.hpp"
#include "skillquest/game/base/thing/character/NonPlayerCharacter.hpp"

namespace sq::sh {
  typedef std::shared_ptr< skillquest::game::base::thing::character::Character > Character;
  typedef std::shared_ptr< skillquest::game::base::thing::character::PlayerCharacter > PlayerCharacter;
  typedef std::shared_ptr< skillquest::game::base::thing::character::NonPlayerCharacter > NonPlayerCharacter;
}