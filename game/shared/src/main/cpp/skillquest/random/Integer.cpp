/**
 * @author omnisudo
 * @date 2023.11.24
 */

#include "skillquest/random/Integer.hpp"

namespace skillquest::random {
	Integer::Integer () {
		_randomizer = std::mt19937( std::random_device()() );
	}
	
	std::uint64_t Integer::uuid () {
		std::uniform_int_distribution< std::uint64_t > dist(
				std::numeric_limits< std::uint64_t >::min(),
				std::numeric_limits< std::uint64_t >::max()
		);
		return dist( _randomizer );
	}
}