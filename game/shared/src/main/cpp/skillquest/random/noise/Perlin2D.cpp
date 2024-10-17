#include "skillquest/random/noise/Perlin2D.hpp"
#include "skillquest/random/Double.hpp"

namespace skillquest::random {
	Perlin2D::Perlin2D ( long long int seed ) : _seed( seed ) {
	
	}
	
	Perlin2D::~Perlin2D () {
	
	}
	
	float curve ( float x ) {
		return ( 3 - 2 * x ) * x * x;
	}
	
	float lerp ( float a, float b, float delta ) {
		return a + delta * ( b - a );
	}
	
	float Perlin2D::at ( glm::vec2 pos ) {
		auto rounded = glm::ivec2( floorf( pos.x ), floorf( pos.y ) );
		auto iaa = rounded + glm::ivec2( 0, 0 );
		auto iba = rounded + glm::ivec2( 1, 0 );
		auto iab = rounded + glm::ivec2( 0, 1 );
		auto ibb = rounded + glm::ivec2( 1, 1 );
		
		auto raa = glm::vec2( random::Double( glm::vec3{ iaa.x, iaa.y, _seed } ).vec2() );
		auto rba = glm::vec2( random::Double( glm::vec3{ iba.x, iba.y, _seed } ).vec2() );
		auto rab = glm::vec2( random::Double( glm::vec3{ iab.x, iab.y, _seed } ).vec2() );
		auto rbb = glm::vec2( random::Double( glm::vec3{ ibb.x, ibb.y, _seed } ).vec2() );
		
		auto offset = pos - glm::vec2( rounded );
		
		auto daa = glm::dot( raa, offset - glm::vec2( 0, 0 ) );
		auto dba = glm::dot( rba, offset - glm::vec2( 1, 0 ) );
		auto dab = glm::dot( rab, offset - glm::vec2( 0, 1 ) );
		auto dbb = glm::dot( rbb, offset - glm::vec2( 1, 1 ) );
		
		auto la = lerp( daa, dba, curve( offset.x ) );
		auto lb = lerp( dab, dbb, curve( offset.x ) );
		
		return lerp( la, lb, curve( offset.y ) );
	}
}