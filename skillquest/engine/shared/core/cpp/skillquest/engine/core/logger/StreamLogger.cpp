/**
 * @author omnisudo
 * @date Jun 19 2024
 **/

#include "StreamLogger.hpp"
#include <string_view>
#include <chrono>
#include <iomanip>
#include <sstream>

namespace skillquest::engine::core::logger {
	StreamLogger::StreamLogger ( std::ostream& stream ) : _stream( stream ) {}
	
	StreamLogger::~StreamLogger () = default;
	
	void StreamLogger::print ( std::string_view level, std::string_view message ) {
		auto now = std::chrono::system_clock::now();
		auto timer = std::chrono::system_clock::to_time_t( now );
		auto currentTime = *std::localtime( &timer );
		auto timestamp = std::put_time( &currentTime, "%m.%d.%Y | %H:%M:%S" );
		auto frame = std::stacktrace::current()[ 3 ];

		std::ostringstream oss;
		oss << std::string( "[ " ) << timestamp << " | " << level << " | " << frame.description() << " ] " << message;
		
		_stream << oss.str() << std::endl;
	}
}
