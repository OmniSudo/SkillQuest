/**
 * @author omnisudo
 * @data Jun 19 2024
 **/

#include "skillquest/event/EventBus.hpp"
#include <memory>

namespace skillquest::factory::event {
	std::shared_ptr< skillquest::event::EventBus > build ();
}
