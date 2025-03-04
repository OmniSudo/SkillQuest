/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#include "skillquest/shared.h++"
#include "skillquest/engine/core/logger/StreamLogger.hpp"

namespace skillquest {
    Shared::Shared() { Logger = new skillquest::engine::core::logger::StreamLogger(std::cout); }

    Shared *SH() {
        if (Shared::INSTANCE == nullptr) { Shared::INSTANCE = new Shared(); }

        return Shared::INSTANCE;
    }
}
