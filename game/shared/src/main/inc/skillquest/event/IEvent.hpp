/**
 * @author omnisudo
 * @data 2023.07.15
 */

#pragma once

#include <chrono>
#include "skillquest/property.hpp"

namespace skillquest::event {
	class IEvent {
	public:
	property( consumed, bool, public, public );
	
	property(
			timestamp,
			std::chrono::time_point< std::chrono::system_clock COMMA std::chrono::nanoseconds >,
			public,
			none
	);
	
	public:
		IEvent () : _timestamp( std::chrono::system_clock::now() ), _consumed( false ) {}
		
		virtual ~IEvent () = default;
	};
}
