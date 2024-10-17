#pragma once

#include <string>
#include "skillquest/string.hpp"
#include <utility>

namespace skillquest::network {
	/**
	 * A networking address
	 * @author  OmniSudo
	 * @date    25.11.21
	 * @date	27.02.23
	 */
	struct Address : public skillquest::util::ToString {
	private:
		std::string _ip;
		
		unsigned short _port;
	
	public:
		Address ( std::string ip, unsigned short port ) : _ip( std::move( ip ) ), _port( port ) {}
		
		Address ( std::string address ) {
			auto find = address.find( ":" );
			if ( find != std::string::npos ) {
				_ip = address.substr( 0, find );
				_port = std::stoi( address.substr( find + 1 ) );
			} else {
				_ip = "0.0.0.0";
				_port = 0;
			}
		}
	
	public:
		auto operator<=> ( const Address& a ) const = default;
	
	public:
		inline std::string ip () const {
			return _ip;
		}
		
		inline unsigned short port () const {
			return _port;
		}
	
	public:
		std::string toString () const override {
			return ip() + ":" + util::toString( port() );
		}
	};
}
