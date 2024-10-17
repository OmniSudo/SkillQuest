/**
 * @author omnisudo
 * @data 2023.02.20
 */

#pragma once

#include "skillquest/logger/ILogger.hpp"
#include <ostream>
#include "skillquest/sh.export.hpp"

namespace skillquest::logger {
	class StreamLogger : public ILogger {
	public:
		StreamLogger ( std::ostream& stream );
		
		~StreamLogger () override;
	
	protected:
		auto print ( std::string_view level, std::string_view message ) -> void final;
		
		std::ostream& _stream;
	};
}
