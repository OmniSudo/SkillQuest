/**
 * @author OmniSudo
 * @date 2023.02.24
 * @date 2023.07.20
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/logger.hpp"

namespace skillquest::factory::network {
	auto build () -> std::shared_ptr< skillquest::network::NetworkController >;
}