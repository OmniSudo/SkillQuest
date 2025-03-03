/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#pragma once

#include <string>
#include "Event.h++"

namespace skillquest::engine::core {
    class Application;

    class Addon {
    public:
        Addon();

        ~Addon();

    public:
        virtual std::string name() {
            return "";
        }

        virtual std::string description() {
            return "";
        }

        virtual std::string version() {
            return "";
        }

        virtual std::string author() {
            return "";
        }

        virtual std::string license() {
            return "";
        }

        virtual std::string icon() {
            return "";
        }

        virtual std::string category() {
            return "";
        }

    public:
        Application* application() {
            return _application;
        }

        Addon* application(Application* app) {
            if (_application == app) {
                return this;
            }

            if (_application != nullptr) {
                // TODO: unmounted
            }

            _application = app;

            if (_application != nullptr) {
                // TODO: mounted
            }

            return this;
        }

    private:
        Application* _application;
    };
} // namespace skillquest::engine::core
