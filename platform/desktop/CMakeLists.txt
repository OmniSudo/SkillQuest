cmake_minimum_required(VERSION 3.19)
include_guard()
project(GENERATED)
include(${PROJECT_SOURCE_DIR}/project.cmake)

PROJECT_TEMPLATE(
    EXECUTABLE
    MODULE skillquest.game
    PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main
    SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/cpp
    INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/inc
    GLOB_SOURCES
    GLOB_INCLUDES
    SHARED_LINK
        skillquest.shared
        skillquest.client
)


if (NOT EMSCRIPTEN)
    install(TARGETS skillquest.game RUNTIME DESTINATION ${CMAKE_CURRENT_LIST_DIR}/bin )
else()
    install(FILES
            "$<TARGET_FILE_DIR:skillquest.game>/skillquest.game.js"
            "$<TARGET_FILE_DIR:skillquest.game>/skillquest.game.wasm"
            DESTINATION ${CMAKE_CURRENT_LIST_DIR}/bin)
endif()