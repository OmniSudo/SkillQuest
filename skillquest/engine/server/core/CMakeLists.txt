include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

find_package(unofficial-sqlite3 CONFIG REQUIRED)
find_package(PostgreSQL REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.engine.server.core
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        skillquest.engine.shared.api
        skillquest.engine.shared.core
        unofficial::sqlite3::sqlite3
        PostgreSQL::PostgreSQL
)