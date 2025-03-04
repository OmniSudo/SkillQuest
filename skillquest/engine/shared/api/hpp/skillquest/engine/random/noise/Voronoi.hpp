#pragma once

#include <glm/vec2.hpp>
#include <glm/gtx/norm.hpp>
#include <vector>
#include <set>
#include "skillquest/random/Delaunator.hpp"
#include <map>

#include <unordered_map>

#define GLM_ENABLE_EXPERIMENTAL

#include <glm/gtx/hash.hpp>

namespace skillquest::random {
	class Voronoi {
	public:
		struct Cell;
		
		struct Point {
			glm::vec2 position;
			std::vector< Point* > neighbors;
			std::vector< Cell* > cells;
		};
		
		struct Cell {
			std::vector< Point* > points;
			std::vector< Cell* > neighbors;
			
			std::vector< glm::vec2 > vertexes;
			std::vector< int > indexes;
			
			bool border;
		};
		
		std::map< std::size_t, Point* > vertices;
		std::map< std::size_t, Cell* > cells;
		
		std::vector< glm::vec2 > points;
		
		delaunator::Delaunator* delaunay;
		
		typedef std::uint64_t Seed;
	
	public:
		Voronoi ( std::vector< glm::vec2 > points );
		
		Voronoi ( Seed seed, glm::vec2 min, glm::vec2 max, double density );
		
		virtual ~Voronoi ();
	
	private:
		auto calculate ( std::vector< glm::vec2 > points ) -> void;
		
		/**
		 * Gets the IDs of the points comprising the given triangle. Taken from {@link https://mapbox.github.io/delaunator/#triangle-to-points| the Delaunator docs.}
		 * @param t The index of the triangle
		 * @returns [number, number, number] The IDs of the points comprising the given triangle.
		 */
		auto pointsOfTriangle ( std::size_t t ) -> std::vector< std::size_t >;
		
		auto trianglesAdjacentToTriangle ( std::size_t t ) -> std::vector< std::size_t >;
		
		auto edgesAroundPoint ( std::size_t start ) -> std::vector< std::size_t >;
		
		auto triangleCenter ( std::size_t index ) -> glm::vec2;
		
		static auto edgesOfTriangle ( std::size_t index ) -> std::vector< std::size_t >;
		
		static auto triangleOfEdge ( std::size_t edge ) -> std::size_t;
		
		static auto nextHalfedge ( std::size_t edge ) -> std::size_t;
		
		static auto circumcenter ( glm::vec2 a, glm::vec2 b, glm::vec2 c ) -> glm::vec2;
	};
}