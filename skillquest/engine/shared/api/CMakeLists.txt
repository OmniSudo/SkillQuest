include_guard()
project(GENERATED)
include("${PROJECT_SOURCE_DIR}/project.cmake")

find_package(nlohmann_json CONFIG REQUIRED)
find_package(OpenSSL REQUIRED)
find_package(glm CONFIG REQUIRED)

PROJECT_TEMPLATE(
        MODULE skillquest.engine.shared.api
        PROJECT_DIR ${CMAKE_CURRENT_LIST_DIR}
        SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR}/cpp
        INCLUDE_DIR ${CMAKE_CURRENT_LIST_DIR}/hpp
        GLOB_SOURCES
        GLOB_INCLUDES
        SHARED_LINK
        nlohmann_json::nlohmann_json
        OpenSSL::Crypto
        glm::glm
)