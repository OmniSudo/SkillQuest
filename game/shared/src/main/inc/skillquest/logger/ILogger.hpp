/**
 * @author omnisudo
 * @data 2023.02.20
 */

#pragma once

#include "skillquest/string.hpp"
#include <vector>
#include <regex>

namespace skillquest::logger {
	class ILogger {
	protected:
		std::vector< std::shared_ptr< ILogger > > _subloggers;
		
		ILogger () = default;
	
	public:
		virtual ~ILogger () = default;
	
	public:
		template < typename ... TArgs >
		auto trace ( std::string format, TArgs ... args ) -> void {
			log( " trace ", format, args... );
		}
		
		template < typename ... TArgs >
		auto info ( std::string format, TArgs ... args ) -> void {
			log( " info  ", format, args... );
		}
		
		template < typename ... TArgs >
		auto warning ( std::string format, TArgs ... args ) -> void {
			log( "warning", format, args... );
		}
		
		template < typename ... TArgs >
		auto error ( std::string format, TArgs ... args ) -> void {
			log( " error ", format, args... );
		}
		
		template < typename ... TArgs >
		auto fatal ( std::string format, TArgs ... args ) -> void {
			log( " fatal ", format, args... );
		}
	
	private:
		template < typename ... TArgs >
		auto log ( std::string_view level, std::string format, TArgs ... args ) -> void {
			std::string formattedMsg = format;
			std::vector< std::string > data = util::toStrings( args... );
			for ( int i = 0; i < data.size(); i++ ) {
				std::regex expression( "\\{" + util::toString( i ) + "\\}" );
				std::smatch smatch;
				std::regex_search( formattedMsg, smatch, expression );
				if ( !smatch.empty() ) formattedMsg = std::string( smatch.prefix() ) + data[ i ] +
													  std::string( smatch.suffix() );
			}
			
			for ( auto logger: _subloggers ) {
				logger->print( level, formattedMsg );
			}
			
			print( level, formattedMsg );
		}
	
	protected:
		virtual auto print ( std::string_view level, std::string_view message ) -> void = 0;
	};
}
