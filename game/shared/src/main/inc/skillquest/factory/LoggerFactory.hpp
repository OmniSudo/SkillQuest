/**
 * @author omnisudo
 * @data Jun 19 2024
 **/

#include "skillquest/logger/ILogger.hpp"
#include <memory>
#include <ostream>

namespace skillquest::factory::logger {
	std::shared_ptr< skillquest::logger::ILogger > build ( std::ostream& stream );
}
