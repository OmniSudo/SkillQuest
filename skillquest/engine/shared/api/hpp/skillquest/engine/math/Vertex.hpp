/**
 * @author omnisudo
 * @date 2024.03.09
 */

#pragma once

#include "skillquest/glm.hash.hpp"
#include <glm/glm.hpp>

namespace skillquest::math {
	struct Vertex {
		glm::vec3 position;
		glm::vec2 uv;
		glm::vec4 color;
		glm::vec3 normal;
	};
}
