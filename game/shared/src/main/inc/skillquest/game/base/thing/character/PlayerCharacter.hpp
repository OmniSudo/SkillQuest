/**
 * @author omnisudo
 * @date 2024.07.26
 */

#pragma once

#include "Character.hpp"
#include "skillquest/stuff.thing.hpp"
#include "skillquest/network.hpp"

namespace skillquest::game::base::thing::character {
    class PlayerCharacter : public Character {
    public:
        struct CreateInfo {
            const Character::CreateInfo &character;
            std::string name;
        };

        property(name, std::string, public, public_ptr);

    public:
        explicit PlayerCharacter(const CreateInfo &info)
            : Character{info.character},
              _name{info.name} {
        }

        ~PlayerCharacter() override = default;
    };
}
