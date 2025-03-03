/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#include "skillquest\engine\core\Addon.h++"

namespace skillquest::engine::core {
Addon::~Addon() {
  _application = nullptr;
}
} // namespace skillquest::engine::core