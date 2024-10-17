/**
 * @author omnisudo
 * @date 2024.07.29
 */

#include "skillquest/factory/NetworkFactory.hpp"
#include "skillquest/platform.hpp"
#include "skillquest/logger/StreamLogger.hpp"
#include <memory>

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include "skillquest/network/socket/Controller.hpp"

#elif defined( PLATFORM_WINDOWS )
#include "skillquest/network/winsock/Controller.hpp"
#endif

namespace skillquest::factory::network {
	auto build () -> std::shared_ptr< skillquest::network::NetworkController > {
		return std::static_pointer_cast< skillquest::network::NetworkController >(
#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )
				std::make_shared< skillquest::network::socket::NetworkController >()
#elif defined( PLATFORM_WINDOWS )
				std::make_shared< skillquest::network::winsock::NetworkController >()
#endif
		);
	}
}