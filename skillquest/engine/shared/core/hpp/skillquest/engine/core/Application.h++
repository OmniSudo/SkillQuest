/**
 * @author omnisudo
 * @date 12.24.2024
 */

#pragma once

#include <vector>
#include <string>
#include "Addon.h++"

namespace skillquest::engine::core {
    class Application {
    public:
        Application(int argc, const char **argv);

        ~Application();

    public:
        std::vector<std::string> args = {};
    };
} // namespace skillquest::engine::core
