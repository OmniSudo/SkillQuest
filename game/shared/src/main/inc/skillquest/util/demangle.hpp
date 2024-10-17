#pragma once

#pragma once

#include <string>
#include <typeindex>

namespace skillquest::util {
	/**
	 * Demangle a type name
	 * @param type Type to cast the type_name of
	 * @return Name of type
	 */
	std::string demangle ( const std::type_index& type );
	
	/**
	 * Demangle a type name
	 * @param type Type to cast the type_name of
	 * @return Name of type
	 */
	std::string demangle ( const std::string type );
}
