#pragma once

#include <functional>
#include <iterator>

namespace skillquest::util {
	class HasHash {
	public:
		virtual auto hash () const -> std::size_t = 0;
	};
	
	template < typename... >
	struct hash;
	
	template < typename T >
	struct hash< T > : public std::hash< T > {
		using std::hash< T >::hash;
	};
	
	template < typename T, typename... Rest >
	struct hash< T, Rest... > {
		std::size_t operator() ( const T& v, const Rest& ... rest ) {
			std::size_t seed = hash< Rest... >{}( rest... );
			seed ^= hash< T >{}( v ) + 0x9e3779b9 + ( seed << 6 ) + ( seed >> 2 );
			return seed;
		}
	};
	
	template < typename T  >
	struct hash< std::vector< T > > {
		std::size_t operator() ( const typename std::vector< T >::iterator& begin, const typename std::vector< T >::iterator& end ) {
			if ( begin == end ) return 0;
			
			auto i = begin;
			std::size_t seed = hash< T >{}( *begin++ );
			for ( ; i != end; i++ ) {
				seed ^= hash< T >{}( *i ) + 0x9e3779b9 + ( seed << 6 ) + ( seed >> 2 );
			}
			return seed;
		}
	};
}

#define enable_hash( type ) template<> struct std::hash<type> { \
    std::size_t operator() (skillquest::core::HasHash const& v) const noexcept { \
        return v.hash();                      \
    } \
};
