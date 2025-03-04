include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

PROJECT_TEMPLATE(
        MODULE skillquest.addon.${addon}.server
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.engine.shared.api
        skillquest.engine.shared.core
        skillquest.engine.server.core

        skillquest.addon.${addon}.shared
)