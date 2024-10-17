/**
 * @author  OmniSudo
 * @date    2023.09.11
 */

#pragma once

#include <nlohmann/json.hpp>

using namespace nlohmann;

namespace skillquest::util {
	class JsonSerializeable {
	public:
		JsonSerializeable ( json data ) {
		
		}
		
		json serialize () {
			return json{};
		}
	};
}