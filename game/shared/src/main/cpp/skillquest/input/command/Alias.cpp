/**
 * @author omnisudo
 * @data 2023.02.21
 */

#include "skillquest/input/command/Alias.hpp"
#include "skillquest/input/command/Commands.hpp"

namespace skillquest::input::command {
	Alias::Alias (
			decltype( _commands ) commands,
			decltype( _input ) input,
			decltype( _output ) output
	) : _commands( commands ), _input( std::move( input ) ), _output( std::move( std::move( output ) ) ) {}
	
	auto Alias::operator() () -> void {
		for ( const auto& i: _output ) {
			_commands->process( i );
		}
	}
}
