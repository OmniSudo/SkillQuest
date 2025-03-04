/**
 * @author omnisudo
 * @date 2023.11.24
 */

#include "skillquest/engine/random/Double.hpp"
#include "skillquest/base64.hpp"

namespace skillquest::random {
	Double::Double () {
		_randomizer = std::mt19937( std::random_device()() );
	}
	
	auto Double::number () -> double {
		std::uniform_real_distribution dist( -1.f, 1.f );
		auto ret = dist( _randomizer );
		return ret;
	}
	
	glm::vec2 Double::vec2 () {
		std::uniform_real_distribution dist( -1.f, 1.f );
		auto ret = glm::vec2( dist( _randomizer ), dist( _randomizer ) );
		return glm::normalize( ret );
	}
	
	glm::vec3 Double::vec3 () {
		std::uniform_real_distribution dist( -1.f, 1.f );
		auto ret = glm::vec3( dist( _randomizer ), dist( _randomizer ), dist( _randomizer ) );
		return glm::normalize( ret );
	}
	
	glm::vec4 Double::vec4 () {
		std::uniform_real_distribution dist( -1.f, 1.f );
		auto ret = glm::vec4( dist( _randomizer ), dist( _randomizer ), dist( _randomizer ), dist( _randomizer ) );
		return glm::normalize( ret );
	}
	
	auto Double::within ( double min, double max ) -> double {
		std::uniform_real_distribution distX( min, max );
		double d = distX( _randomizer );
		return d;
	}
	
	glm::vec2 Double::within ( glm::vec2 min, glm::vec2 max ) {
		std::uniform_real_distribution distX( min.x, max.x );
		std::uniform_real_distribution distY( min.y, max.y );
		auto vec = glm::vec2( distX( _randomizer ), distY( _randomizer ) );
		return vec;
	}
	
	glm::vec3 Double::within ( glm::vec3 min, glm::vec3 max ) {
		std::uniform_real_distribution distX( min.x, max.x );
		std::uniform_real_distribution distY( min.y, max.y );
		std::uniform_real_distribution distZ( min.z, max.z );
		return glm::vec3( distX( _randomizer ), distY( _randomizer ), distZ( _randomizer ) );
	}
	
	glm::vec4 Double::within ( glm::vec4 min, glm::vec4 max ) {
		std::uniform_real_distribution distX( min.x, max.x );
		std::uniform_real_distribution distY( min.y, max.y );
		std::uniform_real_distribution distZ( min.z, max.z );
		std::uniform_real_distribution distW( min.w, max.w );
		return glm::vec4( distX( _randomizer ), distY( _randomizer ), distZ( _randomizer ), distW( _randomizer ) );
	}
	
	auto
	Double::pointCloud ( std::uint64_t seed, glm::vec2 min, glm::vec2 max, float delta ) -> std::vector< glm::vec2 > {
		std::vector< glm::vec2 > points;
		auto tmp = min;
		min = glm::min( min, max );
		max = glm::max( tmp, max );
		
		double x = min.x;
		while ( x + delta <= max.x ) {
			double y = min.y;
			while ( y + delta <= max.y ) {
				points.emplace_back(
						Double(
								glm::vec3{
										seed,
										std::hash< glm::vec2 >()( { x, y } ),
										std::hash< glm::vec2 >()( { x + delta, y + delta } ),
								}
						).within(
								{ x, y },
								{ x + delta, y + delta }
						)
				);
				y += delta;
			}
			x += delta;
		}
		return points;
	}
}