/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/event.hpp"
#include "skillquest/property.hpp"
#include "skillquest/game/base/thing/character/PlayerCharacter.hpp"

namespace skillquest::game::base::event {
    class PlayerJoinedEvent : public skillquest::event::IEvent {
    property(
            player,
            std::shared_ptr< thing::character::PlayerCharacter >,
            public, public_ptr
    );
    };
}