/**
 * @author omnisudo
 * @date 12.24.2024
 */

#pragma once

#include "Addon.h++"
#include <string>
#include <vector>

namespace skillquest::engine::core {
class Application {
public:
  Application(int argc, const char **argv);

  ~Application();

public:
  std::vector<std::string> args = {};

public:
  Application *mount(Addon *addon);

  Application *unmount(Addon *addon);

public:
  Application(const Application &) = delete;
  Application(Application &&) = delete;
  Application &operator=(const Application &) = delete;
  Application &operator=(Application &&) = delete;
};
} // namespace skillquest::engine::core
