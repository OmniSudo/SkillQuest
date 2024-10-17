#pragma once

#include "skillquest/json.hpp"
#include "skillquest/property.hpp"
#include "skillquest/random.hpp"
#include "skillquest/event.hpp"
#include "skillquest/util/demangle.hpp"
#include <typeindex>

namespace skillquest::network {
	/**
	 * A packets of information
	 * @author  OmniSudo
	 * @date    29.11.21
	 * @date	27.02.23
	 */
	class IPacket : public event::IEvent {
	public:
	property( type, const std::string, public, none )
	
	protected:
		explicit IPacket ( const std::string type ) : _type( type ) {
		}
		
		explicit IPacket ( const std::type_index type ) : _type( std::string( type.name() ) ) {}
	
	public:
		explicit IPacket ( json data ) : _type( data[ "type" ].get< std::string >() ) {}
	
	public:
		virtual json serialize () const;
		
	};
	
	typedef std::shared_ptr< network::IPacket > Packet;
#define IPacket_INIT network::IPacket{util::demangle(std::string(typeid(decltype(*this)).name()))}
}
