#pragma once

#include <string>

namespace skillquest::convert {
	namespace base64 {
		std::string encode ( const std::string& in );
		
		std::string decode ( const std::string& in );
	}
	namespace hex {
		std::string encode ( const std::string& in );
		
		std::string decode ( const std::string& in );
	}
}