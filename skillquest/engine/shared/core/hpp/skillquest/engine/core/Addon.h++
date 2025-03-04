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
      Addon() = default;

      virtual ~Addon();

    public:
      virtual std::string name() { return ""; }

      virtual std::string description() { return ""; }

      virtual std::string version() { return ""; }

      virtual std::string author() { return ""; }

      virtual std::string license() { return ""; }

      virtual std::string icon() { return ""; }

      virtual std::string category() { return ""; }

    public:
      Event<Application *, Addon *> mounted = {};

      Event<Application *, Addon *> unmounted = {};

    protected:
      virtual void onMounted(Application *application) {}

      virtual void onUnmounted(Application *application) {}

    private:
      inline static void _mounted(Application *application, Addon* addon ) {
        addon->onMounted(application);
        addon->mounted( application, addon );
      }

      inline static void _unmounted(Application *application, Addon* addon ) {
        addon->unmounted(application, addon);
        addon->onUnmounted(application);
      }

    public:
      Application *application() { return _application; }

      Addon *application(Application *app) {
        if (_application == app) {
          return this;
        }

        if (_application != nullptr) {
          _unmounted( _application, this );
        }

        _application = app;

        if (_application != nullptr) {
          _mounted( _application, this );
        }

        return this;
      }

    private:
      Application *_application;
    };
    } // namespace skillquest::engine::core
