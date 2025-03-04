#pragma once

#include <ostream>
#include <string>
#include <iostream>
#include <vector>
#include <type_traits>

namespace skillquest::util {
	/**
	 * ToString pattern
	 * @author OmniSudo
	 * @date 11.17.2018
	 */
	class ToString {
	public:
		/**
		 * Convert class to string
		 * @return converted class
		 */
		virtual std::string toString () const = 0;
		
		virtual ~ToString () = default;
		
		auto operator<=> ( const ToString& o ) const = default;
	};
	
	template < typename T >
	typename std::enable_if< std::is_base_of< ToString, T >::value, std::string >::type toString ( T const& obj ) {
		return obj.toString();
	}
	
	template < typename T >
	typename std::enable_if<
			not std::is_base_of< ToString, T >::value and std::is_fundamental< T >::value, std::string >::type
	toString ( T const& obj ) {
		return std::to_string( obj );
	}
	
	template < typename T >
	typename std::enable_if<
			not std::is_base_of< ToString, T >::value and not std::is_fundamental< T >::value, std::string >::type
	toString ( T const& obj ) {
		return std::string( obj );
	}
	
	template < typename ... Args >
	std::vector< std::string > toStrings ( Args... args ) {
		std::vector< std::string > ret;
		(ret.push_back( toString( args ) ), ...  );
		return ret;
	}
}

/**
 * Print out ToString via ostream
 * @param out The stream
 * @param obj The toString-able object
 * @return The stream
 */
std::ostream& operator<< ( std::ostream& out, const skillquest::util::ToString& obj );

