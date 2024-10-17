cmake_minimum_required(VERSION 3.19)
include_guard()
project(GENERATED)
include(${PROJECT_SOURCE_DIR}/project.cmake)

add_subdirectory(${PROJECT_SOURCE_DIR}/lib/imgui-cmake)

find_package(PNG REQUIRED)

if ( NOT EMSCRIPTEN )
    find_package(glfw3 CONFIG REQUIRED)
    find_package(OpenGL REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.client
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/inc
        OUTPUT_DIR ${CMAKE_CURRENT_LIST_DIR}/bin
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.shared
        OpenGL::GL
        glfw
        imgui
        PNG::PNG
)
else()
    PROJECT_TEMPLATE(
            MODULE skillquest.client
            PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main
            SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/cpp
            INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/inc
            GLOB_SOURCES
            GLOB_INCLUDES
            SHARED_LINK
            skillquest.shared
            imgui
            PNG::PNG
    )

    target_link_options( skillquest.client PUBLIC -sUSE_GLFW=3 -sASSERTIONS=1 -sWASM=1 -sMAX_WEBGL_VERSION=2)
endif()
