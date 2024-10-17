/**
 * @author omnisudo
 * @data 2023.02.21
 */

#include "skillquest/input/command/Commands.hpp"
#include "skillquest/sh.api.hpp"
#include <future>
#include <fcntl.h>

namespace skillquest::input::command {
	auto Commands::parse ( const std::string& command ) -> std::vector< std::string > {
		auto ret = std::vector< std::string >();
		
		auto length = command.length();
		bool dQuote = false, sQuote = false;
		
		for ( unsigned long long i = 0; i < length; i++ ) {
			auto start = i;
			switch ( command[ i ] ) {
				case '\"':
					dQuote = true;
					
					break;
				case '\'':
					sQuote = true;
					
					break;
			}
			
			unsigned long long argLength;
			if ( dQuote ) {
				i++;
				start++;
				while ( i < length && command[ i ] != '\"' ) i++;
				if ( i < length ) dQuote = false;
				argLength = i - start;
			} else if ( sQuote ) {
				i++;
				start++;
				while ( i < length && command[ i ] != '\'' ) i++;
				if ( i < length ) sQuote = false;
				argLength = i - start;
			} else {
				while ( i < length && command[ i ] != ' ' ) i++;
				argLength = i - start;
			}
			
			ret.push_back( command.substr( start, argLength ) );
		}
		
		if ( dQuote || sQuote ) ret.clear(); // No command = invalid command
		
		return ret;
	}
	
	auto Commands::process ( std::string command ) -> void {
		enqueue( parse( command ) );
	}
	
	Commands::Commands () {
	}
	
	auto Commands::update () -> void {
		// TODO: Non blocking read
	}
	
	Commands::~Commands () {
		running = false;
	}
	
	auto Commands::command (
			const std::string& name,
			std::size_t argc,
			std::function< void ( command::ARGS context ) > callback
	) -> std::shared_ptr< skillquest::input::command::Command > {
		std::scoped_lock l{ _mut };
		
		auto command = std::shared_ptr< Command >(
				new Command{ this, name, argc, callback },
				[ this, name ] ( auto ptr ) -> void {
					std::scoped_lock lock{ _mut };
					enabled_commands.erase( name );
					auto i = this->commands.find( name );
					if ( i != this->commands.end() ) {
						this->commands.erase( i );
					}
					delete ptr;
				}
		);
		
		enabled_commands[ name ] = true;
		
		return ( commands[ name ] = command ).lock();
	}
	
	auto Commands::alias (
			std::string input,
			std::vector< std::string > output
	) -> std::shared_ptr< Alias > {
		std::scoped_lock l{ _mut };
		
		auto alias = std::shared_ptr< Alias >(
				new Alias{ this, input, output },
				[ this, input ] ( auto ptr ) -> void {
					std::scoped_lock lock{ _mut };
					enabled_commands.erase( input );
					auto i = this->aliases.find( input );
					if ( i != this->aliases.end() ) {
						this->aliases.erase( i );
					}
					delete ptr;
				}
		);
		
		enabled_commands[ input ] = true;
		
		return ( aliases[ input ] = alias ).lock();
	}
	
	auto Commands::enqueue ( const std::vector< std::string >& command ) -> void {
		if ( command.empty() ) return;
		
		queue.emplace_back( command );
	}
	
	bool Commands::execute () {
		std::unique_lock l{ _mut };
		
		if ( queue.empty() ) return false;
		
		auto command = queue.front();
		
		queue.erase( queue.begin() );
		
		auto cmd = command[ 0 ];
		auto all = commands.find( "" );
		auto a = aliases.find( cmd );
		auto i = commands.find( cmd );
		
		l.unlock();
		
		std::vector< std::string > args = {};
		if ( command.begin() + 1 != command.end() ) args = std::vector( command.begin() + 1, command.end() );
		
		if ( all != commands.end() && !all->second.expired() ) {
			try {
				if ( !all->second.expired() ) {
					( *all->second.lock() )( command );
				} else {
					commands.erase( all );
				}
			} catch ( std::exception& e ) {
				::sq::shared()->logger()->error( "Unable to finish command '{0}':\n\t{1}", command[ 0 ], e.what() );
				return false;
			}
		}
		
		auto enabled = enabled_commands.find( cmd );
		if ( enabled != enabled_commands.end() && !enabled->second ) {
			::sq::shared()->logger()->trace( "{0} command is disabled!", enabled->first );
			return false;
		}
		
		if ( a != aliases.end() ) {
			if ( !a->second.expired() ) {
				( *a->second.lock().get() )();
				return true;
			} else {
				aliases.erase( a );
			}
		} else {
			if ( all == commands.end() && i == commands.end() ) {
				::sq::shared()->logger()->error( "No such command '{0}'", command[ 0 ] );
				return true;
			}
			
			if ( i != commands.end() && !i->second.expired() ) {
				try {
					if ( !i->second.expired() ) {
						( *i->second.lock() )( command );
					} else {
						commands.erase( i );
					}
				} catch ( std::exception& e ) {
					::sq::shared()->logger()->error( "Unable to finish command '{0}':\n\t{1}", command[ 0 ], e.what() );
					return false;
				}
			}
		}
		return true;
	}
	
	auto Commands::enabled () -> std::map< std::string, bool >& {
		return enabled_commands;
	}
	
	auto Commands::enable ( const std::string& command, bool toggle ) -> Commands& {
		enabled_commands[ command ] = toggle;
		return *this;
	}
	
	void Commands::disable_command ( std::shared_ptr< Command > command ) {
		enabled_commands.erase( command->alias() );
		commands.erase( command->alias() );
	}
	
	void Commands::disable_alias ( std::shared_ptr< Alias > alias ) {
		enabled_commands.erase( alias->input() );
		aliases.erase( alias->input() );
	}
}