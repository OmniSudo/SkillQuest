/**
 * @author  OmniSudo
 * @date    2023.07.20
 */

#pragma once

#if defined( _WIN32 )
#define WIN32_LEAN_AND_MEAN
#define PLATFORM_WINDOWS true
#elif defined( __linux__ )
#define PLATFORM_LINUX true
#elif defined( __APPLE__ )
#define PLATFORM_APPLE true
#elif  defined( __EMSCRIPTEN__ )
#define PLATFORM_WEB true
#else
#define PLATFORM_UNKNOWN
#endif

namespace skillquest::engine {
	enum class Platform {
		WINDOWS, LINUX, MACOS, WEB, UNKNOWN
	};
}
