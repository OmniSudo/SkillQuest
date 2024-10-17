#pragma once

#include <map>
#include <memory>
#include "skillquest/network.hpp"

namespace skillquest::network::socket {
	/**
	 *
	 * @author  OmniSudo
	 * @date    12/6/21
	 */
	class NetworkController : public skillquest::network::NetworkController {
	public:
		explicit NetworkController ();
		
		~NetworkController () override;
		
	};
}