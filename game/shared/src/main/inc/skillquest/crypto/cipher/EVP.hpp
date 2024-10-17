#pragma once

#include <string>

namespace skillquest::crypto::cipher {
	/**
	 * A symmetric key encryption algorithm
	 * @author  OmniSudo
	 * @date    8/27/21
	 */
	class EVP {
	public:
		EVP () = delete;
		
		/**
		 * Encrypt a plaintext message using a key iv pair
		 * @param plaintext The text to try_encrypt using EVP
		 * @param key The cipher key
		 * @param iv Initialization vector
		 * @return Encrypted message
		 */
		static std::string encrypt ( std::string plaintext, std::string key, std::string iv );
		
		/**
		 * Decrypt a plaintext message using a key iv pair
		 * @param cyphertext The encrypted text to decrypt
		 * @param key The cipher key
		 * @param iv Initialization vector
		 * @return Decrypted message
		 */
		static std::string decrypt ( std::string cyphertext, std::string key, std::string iv );
	};
}
