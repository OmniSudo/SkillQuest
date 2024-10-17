/**
 * @author omnisudo
 * @date 2024.09.28
 */

#pragma once

namespace skillquest::math {
    enum class Direction {
        NONE = 0,
        NORTH = 0b1,
        SOUTH = 0b10,
        EAST = 0b100,
        WEST = 0b1000,
        UP = 0b10000,
        DOWN = 0b100000,

        POS_X = EAST,
        NEG_X = WEST,
        POS_Y = NORTH,
        NEG_Y = SOUTH,
        POS_Z = UP,
        NEG_Z = DOWN,

        X = EAST | WEST,
        Y = NORTH | SOUTH,
        Z = UP | DOWN,

        POS = EAST | NORTH | UP,
        NEG = WEST | SOUTH | DOWN,

        ALL = POS | NEG,
    };

    const std::array< Direction, 6 > DIRECTIONS = {
        Direction::POS_X,
        Direction::NEG_X,
        Direction::POS_Y,
        Direction::NEG_Y,
        Direction::POS_Z,
        Direction::NEG_Z,
    };
}