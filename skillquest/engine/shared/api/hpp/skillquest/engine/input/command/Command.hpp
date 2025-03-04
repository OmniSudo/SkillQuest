/**
 * @author  OmniSudo
 * @date    2023.02.21
 */

#pragma once

#include "CommandContext.hpp"
#include <functional>
#include <utility>
#include "skillquest/property.hpp"

namespace skillquest::input::command {
	class Commands;
	
	typedef std::vector< std::string >& ARGS;
	
	/**
	 * A console command
	 */
	class Command {
		friend class Commands;
		/**
		 * Command Controller that handles registration of this command
		 */
	property( commands, Commands*, public, private )
		
		/**
		 * String that this command is invoked by
		 */
	property( alias, std::string, public, private )
		
		/**
		 * Number of arguments
		 */
	property( args, std::size_t, public, private )
		
		/**
		 * The function callback
		 */
	property( callback, std::function< void ( std::vector< std::string >& context ) >, public, private )
	
	protected:
		/**
		 * CTOR
		 */
		Command (
				decltype( _commands ) commands,
				decltype( _alias ) alias,
				decltype( _args ) args,
				decltype( _callback ) callback
		);
	
	public:
		/**
		 * Invoke the command
		 * @param args
		 */
		virtual auto operator() ( ARGS args ) -> void;
		
	};
}