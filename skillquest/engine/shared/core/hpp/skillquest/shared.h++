/**
 * @author omnisudo
 * @date 12.24.2024
 */

#pragma once

#include <skillquest/engine/core/Application.h++>

namespace skillquest {
    struct Shared {
        engine::core::Application *Application = nullptr;

        virtual ~Shared() { delete Application; }

    private:
        inline static Shared *INSTANCE = nullptr;

        friend Shared *SH();
    };

    Shared *SH() {
        if (Shared::INSTANCE == nullptr) {
            Shared::INSTANCE = new Shared();
        }

        return Shared::INSTANCE;
    }
} // namespace skillquest
