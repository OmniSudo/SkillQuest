/**
 * @author  omnisudo
 * @date    2024.12.08
 */

#include <string>
#include <vector>


#include "skillquest/client.h++"
#include "skillquest/server.h++"
#include "skillquest/shared.h++"
#include "skillquest/addon/base/client.h++"
#include "skillquest/addon/base/server.h++"

int main ( int argc, const char** argv ) {
    (skillquest::SH()->Application = new skillquest::engine::core::Application( argc, argv ))
    ->mount(new skillquest::addon::base::core::AddonSkillQuestBaseServer())
    ->mount(new skillquest::addon::base::core::AddonSkillQuestBaseClient());
}
