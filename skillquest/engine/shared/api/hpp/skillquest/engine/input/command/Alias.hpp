/**
 * @author  OmniSudo
 * @date    2023.02.21
 */

#pragma once

#include <memory>
#include <utility>
#include "skillquest/property.hpp"
#include <string>
#include <vector>

namespace skillquest::input::command {
	class Commands;
	
	class Alias {
		friend class Commands;
		/**
		 * Command Controller that handles registration of this command
		 */
	property( commands, Commands*, public, private )
		
		/**
		 * String that this command is invoked by
		 */
	property( input, std::string, public, private )
		
		/**
		 * The function callback
		 */
	property( output, std::vector< std::string >, public, private )
	
	protected:
		/**
		 * CTOR
		 */
		Alias (
				decltype( _commands ) commands,
				decltype( _input ) input,
				decltype( _output ) output
		);
	
	public:
		/**
		 * Invoke the alias
		 */
		auto operator() () -> void;
	};
}