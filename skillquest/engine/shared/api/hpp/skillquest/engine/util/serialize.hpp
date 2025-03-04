/**
 * @author  OmniSudo
 * @date    2023.09.11
 */

#pragma once

#include <nlohmann/json.hpp>

using namespace nlohmann;

namespace skillquest::util {
	class ISerializeable {
	public:
        ISerializeable() = default;

		ISerializeable ( json data ) {

		}
		
		json serialize () {
			return json{};
		}
	};
}