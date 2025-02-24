include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

find_package(PNG REQUIRED)
find_package(imgui CONFIG REQUIRED)
find_package(glfw3 CONFIG REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.engine.client.graphics.api
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.engine.shared.api
        skillquest.engine.shared.core
        skillquest.engine.client.core
        PNG::PNG
        imgui::imgui
        glfw
)