/**
 * @author  OmniSudo
 * @date    2023.02.18
 */

#pragma once

#include "skillquest/property.hpp"

namespace skillquest::application {
    class Engine {
        property(running, bool, public, private)
    public:
        /**
         * CTOR
         * @param createInfo Information used to create the engine
         */
        explicit Engine ();

        /**
         * DTOR
         */
        virtual ~Engine();

        /**
         * Run the engine
         * @return Return code; 0 on normal termination
         */
        auto run () -> int;

        /**
         * Called when the engine starts
         */
        virtual auto onStart () -> void;

        /**
         * Preform a single iteration of the loop
         */
        virtual auto onUpdate () -> void;

        /**
         * Called when the engine stops
         */
        virtual auto onStop () -> void;

        /**
         * Stop the engine
         */
        virtual auto quit () -> void;
    };
}