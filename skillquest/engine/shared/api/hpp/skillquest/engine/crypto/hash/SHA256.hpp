#pragma once

#include <string>
#include <filesystem>

namespace skillquest::crypto::hash::SHA256 {
	std::string string ( const std::string& str );
	
	std::string file ( std::filesystem::path path );
}
