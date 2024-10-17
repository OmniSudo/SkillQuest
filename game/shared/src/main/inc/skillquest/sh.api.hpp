#pragma once

#include <memory>
#include "skillquest/property.hpp"
#include "skillquest/logger.hpp"
#include "skillquest/event.hpp"
#include "skillquest/command.hpp"
#include "skillquest/stuff.hpp"
#include "skillquest/network.hpp"
#include "skillquest/platform.hpp"

namespace skillquest {
    namespace application {
        class Engine;
    }

	class SharedAPI {
    private:
        friend class skillquest::application::Engine;

        inline static SharedAPI* _instance = nullptr;

        SharedAPI () {}

    public:
        static auto instance () -> SharedAPI*& {
            return _instance == nullptr ? ( _instance = new SharedAPI() ) : _instance;
        }

        property( logger, std::shared_ptr< skillquest::logger::ILogger >, public, private )
        property( events, std::shared_ptr< skillquest::event::EventBus >, public, private )
        property( commands, std::shared_ptr< skillquest::input::command::Commands >, public, private )
        property( stuff, std::shared_ptr< skillquest::stuff::IStuff >, public, private )
        property( network, std::shared_ptr< skillquest::network::NetworkController >, public, private )

        ~SharedAPI() = default;
    };
}

namespace sq {
    skillquest::SharedAPI*& shared ();
}
