/**
 * @author  omnisudo
 * @date    2024.12.08
 */

#include "skillquest/shared.h++"
#include "skillquest/server.h++"
#include "skillquest/addon/base/core/AddonSkillQuestBaseServer.h++"

int main(int argc, const char **argv) {
    (skillquest::SH()->Application = new skillquest::engine::core::Application(argc, argv))
            ->mount(new skillquest::addon::base::core::AddonSkillQuestBaseServer());
}
