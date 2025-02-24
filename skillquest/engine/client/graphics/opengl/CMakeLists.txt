include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

find_package(OpenGL REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.engine.client.graphics.opengl
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.engine.shared.api
        skillquest.engine.shared.core
        skillquest.engine.client.core
        skillquest.engine.client.graphics.api
        OpenGL::GL
)