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
	 * A Random Generator
	 * @author  OmniSudo
	 * @date    8/31/21
	 */
	class String {
		std::mt19937 _randomizer;
	
	public:
		/**
		 * Random seed
		 */
		String ();
		
		explicit String ( unsigned long long seed );
		
		template < typename TSeed >
		String ( TSeed seed ) {
			auto i = std::hash< TSeed >()( seed );
			std::seed_seq seq{ i };
			_randomizer = std::mt19937( seq );
		}
		
		virtual ~String () = default;
	
	public:
		std::string length ( size_t length );
		
		auto uid () -> std::string;
		
	};
}