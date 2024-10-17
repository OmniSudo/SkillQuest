/**
 * @author omnisudo
 * @date Jun 19 2024
 **/

#include "skillquest/factory/EventBusFactory.hpp"
#include "skillquest/event.hpp"
#include <memory>

namespace skillquest::factory::event {
	std::shared_ptr< skillquest::event::EventBus > build () {
		return std::make_shared< skillquest::event::EventBus >();
	}
}
