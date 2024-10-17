/**
 * @author omnisudo
 * @date 6/29/24
 */

#include "skillquest/factory/CommandsFactory.hpp"

namespace skillquest::factory::input::command {
	std::shared_ptr< skillquest::input::command::Commands > build () {
		return std::make_shared< skillquest::input::command::Commands >();
	}
}