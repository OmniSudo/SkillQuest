#pragma once

/**
 * @author  OmniSudo
 * @date    2023.02.02
 */

#include <string>
#include "skillquest/random.hpp"
#include "skillquest/string.hpp"

namespace skillquest::util {
	/**
	 * A UID is a randomly generated ID that is /unique/ because the collision chances are minimal.
	 * Good for one time entity connections between client and servers
	 */
	struct UID : skillquest::util::ToString {
		std::string value;
	
	public:
		UID ( std::string value ) : value( value ) {}
		
		UID ( const UID& other ) : value( other.value ) {}
		
		UID () : value() {}
		
		explicit operator std::string () const {
			return value;
		}
		
		std::string toString () const override {
			return value;
		}
		
		virtual auto operator== ( const UID& other ) const -> bool {
			return value == other.value;
		}

		auto operator<=> ( const UID& other ) const -> auto {
			return value <=> other.value;
		}
	};
}

namespace std {
	template <>
	struct hash< skillquest::util::UID > {
		size_t operator() ( const skillquest::util::UID& x ) const {
			return hash< string >()( ( string ) x );
		}
	};
}