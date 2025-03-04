/**
 * @author omnisudo
 * @date 12.24.2024
 */

#pragma once

#include <skillquest/engine/core/Application.h++>
#include "skillquest/engine/logger/Logger.hpp"

namespace skillquest {
    struct Shared {
        engine::core::Application *Application = nullptr;

        engine::logger::Logger* Logger;

        virtual ~Shared() { delete Application; }

    private:
        Shared ();

        inline static Shared *INSTANCE = nullptr;

        friend Shared *SH();
    };

    Shared *SH();
} // namespace skillquest
