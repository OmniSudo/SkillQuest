#pragma once

#include <exception>
#include <functional>
#include <string>

namespace skillquest::crypto::cipher {
	/**
	 * An issue came up when encrypting or decrypting a message using RSA
	 * @author  OmniSudo
	 * @date    8/27/21
	 */
	class RSAException : public std::bad_function_call {
		std::string _what;
	
	public:
		RSAException ( std::string what ) {
			_what = what;
		}
		
		const char* what () const noexcept override {
			return _what.c_str();
		}
	};
}