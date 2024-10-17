/**
 * @author omnisudo
 * @date 2024.07.29
 */

#include "skillquest/sh.api.hpp"

namespace sq {
	skillquest::SharedAPI*& shared () {
		return skillquest::SharedAPI::instance();
	}
}