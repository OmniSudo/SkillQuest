/**
 * @author omnisudo
 * @date 6/29/24
 */

#include "skillquest/input/command/Command.hpp"

namespace skillquest::input::command {
	Command::Command (
			decltype( _commands ) commands, decltype( _alias ) alias, decltype( _args ) args,
			decltype( _callback ) callback
	) : _commands( commands ), _alias( std::move( alias ) ), _args( args ),
		_callback( std::move( callback ) ) {}
	
	auto Command::operator() ( ARGS args ) -> void {
		_callback( args );
	}
}