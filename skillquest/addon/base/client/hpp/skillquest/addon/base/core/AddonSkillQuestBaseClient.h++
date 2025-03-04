/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#pragma once
#include "skillquest/addon/base/shared.h++"

namespace skillquest::addon::base::core {
    class AddonSkillQuestBaseClient : public AddonSkillQuestBaseShared {
        public:

    protected:
        virtual void onMounted(engine::core::Application *application) override;

        virtual void onUnmounted(engine::core::Application *application) override;
    };
}
