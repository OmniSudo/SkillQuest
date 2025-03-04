#pragma once

#include <random>
#include <glm/glm.hpp>

#define GLM_ENABLE_EXPERIMENTAL

#include <glm/gtx/hash.hpp>

namespace skillquest::random {
	/**
	 * A Random Generator
	 * @author  OmniSudo
	 * @date    8/31/21
	 */
	class Double {
		std::mt19937 _randomizer;
	
	public:
		/**
		 * Random seed
		 */
		Double ();
		
		template < typename TSeed >
		Double ( TSeed seed ) {
			auto i = std::hash< TSeed >()( seed );
			std::seed_seq seq{ i };
			_randomizer = std::mt19937( seq );
		}
		
		~Double () = default;
	
	public:
		auto number () -> double;
		
		auto vec2 () -> glm::vec2;
		
		auto vec3 () -> glm::vec3;
		
		auto vec4 () -> glm::vec4;
		
		auto within ( double min, double max ) -> double;
		
		auto within ( glm::vec2 min, glm::vec2 max ) -> glm::vec2;
		
		auto within ( glm::vec3 min, glm::vec3 max ) -> glm::vec3;
		
		auto within ( glm::vec4 min, glm::vec4 max ) -> glm::vec4;
		
		static auto
		pointCloud ( std::uint64_t seed, glm::vec2 min, glm::vec2 max, float delta ) -> std::vector< glm::vec2 >;
		
	};
}
