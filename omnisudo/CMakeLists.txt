include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

find_package(Lua REQUIRED)

PROJECT_TEMPLATE(
        MODULE omnisudo
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        ${LUA_LIBRARIES}
)

target_include_directories(omnisudo PUBLIC ${LUA_INCLUDE_DIR})