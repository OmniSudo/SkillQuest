/**
 * @author  OmniSudo
 * @date    2023.02.20
 */

#pragma once

#include "Command.hpp"
#include "Alias.hpp"
#include "CommandContext.hpp"
#include <map>
#include <thread>
#include <mutex>
#include <memory>

namespace skillquest::input::command {
	class Commands {
	public:
		Commands ();
	
	public:
		virtual ~Commands ();
		
		static auto parse ( const std::string& command ) -> std::vector< std::string >;
		
		auto process ( std::string command ) -> void;
		
		virtual auto enabled () -> std::map< std::string, bool >&;
		
		virtual auto enable ( const std::string& command, bool toggle ) -> Commands&;
		
		virtual auto enqueue ( const std::vector< std::string >& command ) -> void;
		
		virtual auto command (
				const std::string& name, std::size_t argc,
				std::function< void ( command::ARGS& context ) > callback
		) -> std::shared_ptr< Command >;
		
		auto alias ( std::string input, std::vector< std::string > output ) -> std::shared_ptr< Alias >;
		
		virtual auto disable_command ( std::shared_ptr< Command > command ) -> void;
		
		virtual auto disable_alias ( std::shared_ptr< Alias > alias ) -> void;
		
		virtual auto update () -> void;
		
		virtual auto execute () -> bool;
	
	private:
		std::map< std::string, bool > enabled_commands;
		
		std::map< std::string, std::weak_ptr< skillquest::input::command::Command > > commands;
		
		std::map< std::string, std::weak_ptr< skillquest::input::command::Alias > > aliases;
		
		std::vector< std::vector< std::string > > queue;
		
		std::thread read;
		
		bool running = true;
		
		std::mutex _mut;
		
	};
}