/**
 * @author omnisudo
 * @data 2023.02.20
 */

#pragma once

#include "skillquest/engine/logger/Logger.hpp"
#include <ostream>

namespace skillquest::engine::core::logger {
	class StreamLogger : public engine::logger::Logger {
	public:
		StreamLogger ( std::ostream& stream );
		
		~StreamLogger () override;
	
	protected:
		auto print ( std::string_view level, std::string_view message ) -> void final;
		
		std::ostream& _stream;
	};
}
