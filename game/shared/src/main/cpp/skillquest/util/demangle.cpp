/**
 * @author  OmniSudo
 * @date    04.02.2022
 */
#include "skillquest/util/demangle.hpp"
#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include <cxxabi.h>

namespace skillquest::util {
	std::string demangle ( const std::type_index& type ) {
		return demangle( type.name() );
	}
	
	std::string demangle ( const std::string type ) {
		int status = 0;
		auto realname = abi::__cxa_demangle( type.c_str(), 0, 0, &status );
		
		if ( status != 0 ) return "";
		
		auto ret = std::string( realname );
        delete realname;
        return ret;
	}
}
#elif defined( PLATFORM_WINDOWS )
#include <windows.h>
#include <dbghelp.h>
#pragma comment(lib,"dbghelp.lib")

#include <iostream>

namespace skillquest::core {
	std::string demangle ( const std::type_index& type ) {
		return demangle( type.name() );
	}
	
	std::string demangle ( const std::string type ) {
		int status = 0;
		char *realname = (char*)malloc(1024 * sizeof(char));
		::UnDecorateSymbolName( type.c_str(), realname, 1024, UNDNAME_COMPLETE );
		auto str = std::string( realname );
		delete[] realname;
		std::cout << str << std::endl;
		return str;
	}
}
#endif