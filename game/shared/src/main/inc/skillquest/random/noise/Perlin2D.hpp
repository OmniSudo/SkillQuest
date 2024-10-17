#pragma once

#include <glm/vec2.hpp>
#include "Perlin2D.hpp"

namespace skillquest::random {
	/**
	 *
	 * @author  OmniSudo
	 * @date    1/12/22
	 */
	class Perlin2D {
	private:
		long long _seed;
	
	public:
		explicit Perlin2D ( long long seed );
		
		~Perlin2D ();
		
		float at ( glm::vec2 pos );
	
	public:
		inline long long seed () {
			return _seed;
		}
	};
}
