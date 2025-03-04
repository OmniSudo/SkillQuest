/**
 * @author  OmniSudo
 * @date    2023.02.21
 */

#pragma once

#include <memory>
#include "skillquest/logger.hpp"

namespace skillquest::input::command {
	class Command;
	
	class Commands;
	
	/**
	 * A commands context,
	 * Passed as the argument to a command invocation
	 */
	struct CommandContext {
		/**
			 * Ptr to the command module
			 */
		Commands& commands;
		
		/**
		 * The command that was invoked
		 */
		Command& command;
		
		/**
		 * The command arguments
		 */
		std::vector< std::string > args;
		
	};
}