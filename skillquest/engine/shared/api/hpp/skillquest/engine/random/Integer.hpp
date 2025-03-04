/**
 * @author omnisudo
 * @date 2023.11.24
 */

#pragma once


#include <random>
#include <glm/glm.hpp>

#define GLM_ENABLE_EXPERIMENTAL

#include <glm/gtx/hash.hpp>

namespace skillquest::random {
	/**
	 * A Random Generator for Integers
	 * @author  OmniSudo
	 * @date    8/31/21
	 */
	class Integer {
		std::mt19937 _randomizer;
	
	public:
		/**
		 * Random seed
		 */
		Integer ();
		
		template < typename TSeed >
		Integer ( TSeed seed ) {
			auto i = std::hash< TSeed >()( seed );
			std::seed_seq seq{ i };
			_randomizer = std::mt19937( seq );
		}
		
		virtual ~Integer () = default;
	
	public:
		auto uuid () -> std::uint64_t;
	};
}