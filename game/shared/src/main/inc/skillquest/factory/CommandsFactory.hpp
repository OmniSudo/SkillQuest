/**
 * @author omnisudo
 * @date 6/29/24
 */
#pragma once

#include <memory>
#include "skillquest/command.hpp"

namespace skillquest::factory::input::command {
	std::shared_ptr< skillquest::input::command::Commands > build ();
}