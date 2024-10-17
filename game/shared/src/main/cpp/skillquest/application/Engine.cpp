/**
 * @author omnisudo
 * @date 6/28/24
 */

#include "skillquest/application/Engine.hpp"
#include "skillquest/platform.hpp"
#include <functional>

#ifdef PLATFORM_WEB
#include <emscripten.h>
#endif

#include "skillquest/sh.api.hpp"
#include "skillquest/factory/CommandsFactory.hpp"
#include "skillquest/factory/NetworkFactory.hpp"

namespace skillquest::application {
	Engine::Engine () {
	}
	
	Engine::~Engine () {
		this->quit();
	}
	
	std::function< void () > loop;
	
	void execute () {
		loop();
	}
	
	auto Engine::run () -> int {
		onStart();
		
		running( true );
		loop = [ & ] () -> void {
			if ( running() ) {
				this->onUpdate();
			}
		};

#ifdef PLATFORM_WEB
		emscripten_set_main_loop( execute, 0, true );
#else
		while ( running() ) {
			execute();
		}
#endif
		
		onStop();
		
		return 0;
	}
	
	auto Engine::quit () -> void {
		running( false );
	}
	
	auto Engine::onStart () -> void {
		auto sh = sq::shared();
		
		sh->logger( skillquest::factory::logger::build( std::cout ) );
		sh->logger()->trace( "Starting" );
		sh->events( skillquest::factory::event::build() );
		sh->commands( skillquest::factory::input::command::build() );
		sh->stuff( std::shared_ptr< skillquest::stuff::IStuff >( new skillquest::stuff::Stuff{ {} } ) );
		sh->network( skillquest::factory::network::build() );
	}
	
	auto Engine::onUpdate () -> void {
		sq::shared()->commands()->update();
	}
	
	auto Engine::onStop () -> void {
		auto sh = sq::shared();
		
		sh->stuff( nullptr );
		sh->commands( nullptr );
		sh->events( nullptr );
		sh->logger( nullptr );
	}
	
	
}