/**
 * @author omnisudo
 * @date Jun 19 2024
 **/

#include "skillquest/factory/LoggerFactory.hpp"
#include "skillquest/logger/StreamLogger.hpp"
#include <memory>

namespace skillquest::factory::logger {
	std::shared_ptr< skillquest::logger::ILogger > build ( std::ostream& stream ) {
		return std::static_pointer_cast< skillquest::logger::ILogger >(
				std::make_shared< skillquest::logger::StreamLogger >( stream )
		);
	}
}
