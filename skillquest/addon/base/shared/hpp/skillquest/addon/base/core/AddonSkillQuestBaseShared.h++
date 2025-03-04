/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#pragma once
#include "skillquest/addon.h++"

namespace skillquest::addon::base::core {
    class AddonSkillQuestBaseShared : public engine::core::Addon {
        public:

    protected:
        virtual void onMounted(engine::core::Application *application) override;

        virtual void onUnmounted(engine::core::Application *application) override;
    };
}
