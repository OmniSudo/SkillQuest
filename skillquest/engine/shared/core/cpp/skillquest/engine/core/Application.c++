/**
 * @author OmniSudo
 * @date 3/2/2025
 */

#include "skillquest/engine/core/Application.h++"

namespace skillquest::engine::core {
    Application::Application(int argc, const char **argv) {
        for (int i = 1; i < argc; ++i) { args.emplace_back(argv[i]); }
    }

    Application::~Application() {
    }

    Application *Application::mount(Addon *addon) {
        addon->application(this);
        return this;
    }

    Application *Application::unmount(Addon *addon) {
        addon->application(nullptr);
        return this;
    }
}     // namespace skillquest::engine::core
