cmake_minimum_required(VERSION 3.19)
include_guard()
project(GENERATED)
include(${PROJECT_SOURCE_DIR}/project.cmake)

find_package(glm CONFIG REQUIRED)
find_package(nlohmann_json CONFIG REQUIRED)
find_package(OpenSSL REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.shared
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/src/main/inc
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        glm::glm
        nlohmann_json::nlohmann_json
        OpenSSL::Crypto
)
