/**
 * @author  omnisudo
 * @date    2024.12.08
 */

#include "skillquest/shared.h++"
#include "skillquest/client.h++"

int main ( int argc, const char** argv ) {
    skillquest::SH()->Application = new skillquest::engine::core::Application();
}