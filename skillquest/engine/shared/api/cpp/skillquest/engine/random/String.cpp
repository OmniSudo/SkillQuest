/**
 * @author omnisudo
 * @date 2023.11.24
 */

#include "skillquest/engine/random/String.hpp"
#include "skillquest/base64.hpp"

namespace skillquest::random {
	String::String () {
		_randomizer = std::mt19937( std::random_device()() );
	}
	
	std::string String::length ( size_t length ) {
		std::string ret = "";
		std::uniform_int_distribution< short > dist( 0, std::numeric_limits< char >::max() );
		for ( int i = 0; i < length; i++ ) {
			ret += static_cast< char >( dist( _randomizer ) );
		}
		
		return ret;
	}
	
	std::string String::uid () {
		auto g = skillquest::convert::base64::encode( this->length( 21 ) );
		return g;
	}
}