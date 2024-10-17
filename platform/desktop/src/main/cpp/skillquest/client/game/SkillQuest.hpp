/**
 * @author omnisudo
 * @date 6/29/24
 */
#pragma once

#include "skillquest/application/ClientEngine.hpp"

namespace skillquest::client::game {
	class SkillQuest : public application::ClientEngine {
	public:
		auto onStart () -> void override;
		
		auto onStop () -> void override;
		
		auto onUpdate () -> void override;

	};
}