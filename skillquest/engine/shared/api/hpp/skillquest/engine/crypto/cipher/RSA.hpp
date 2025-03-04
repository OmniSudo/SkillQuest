#pragma once

#include <string>

namespace skillquest::crypto::cipher {
	/**
	 * A pair of keys
	 * Public key or private key can be empty, not recommended to do both empty
	 */
	class Keypair {
		std::string _publicKey = "";
		
		std::string _secretKey = "";
	
	public:
		/**
		 * CTOR
		 * @param publicKey
		 * @param privateKey
		 */
		Keypair ( const std::string& publicKey = "", const std::string& privateKey = "" );
	
	public:
		/**
		 * Getter
		 * @return Public key
		 */
		inline std::string public_key() const { return _publicKey; }
		
		/**
		 * Getter
		 * @return Private key
		 */
		inline std::string secret_key() const { return _secretKey; }
	};
	
	/**
	 * RSA asymmetric encryption
	 * Inorder to avoid openssl headers being included an expensive parse of the plaintext public/private keys needs to be done
	 * @author  OmniSudo
	 * @date    8/27/21
	 */
	class RSA {
	public:
		/**
		 * CTOR
		 */
		RSA () = delete;
		
		/**
		 * Generate a key pair
		 * @return Public & private keys
		 */
		static Keypair generate ();
		
		/**
		 * Decrypted with @code decryptPrivate @endcode
		 * --> Encrypted by any
		 *     Decrypted by one
		 * @param plaintext Text
		 * @param keypair Contains public key
		 * @return Cypher text
		 */
		static std::string encryptPublic ( std::string plaintext, Keypair keypair );
		
		/**
		 * Decrypt text encrypted by @code encryptPrivate @endcode
		 *     Encrypted by one
		 * --> Decrypted by any
		 * @param cyphertext Text
		 * @param keypair Contains public key
		 * @return Plain text
		 */
		static std::string decryptPublic ( std::string cyphertext, Keypair keypair );
		
		/**
		 * Decrypted with @code decryptPublic @endcode
		 * --> Encrypted by one
		 *     Decrypted by any
		 * @param plaintext Text
		 * @param keypair Contains private key
		 * @return Cypher text
		 */
		static std::string encryptPrivate ( std::string plaintext, Keypair keypair );
		
		/**
		 * Decrypt text encrypted by @code encryptPublic @endcode
		 *     Encrypted by any
		 * --> Decrypted by one
		 * @param cyphertext Text
		 * @param keypair Contains private key
		 * @return Plain text
		 */
		static std::string decryptPrivate ( std::string cyphertext, Keypair keypair );
		
		/**
		 * Decrypted with @code decryptPrivate @endcode
		 * --> Encrypted by any
		 *     Decrypted by one
		 * @param plaintext Text
		 * @param publicKey key
		 * @return Cypher text
		 */
		inline static std::string encryptPublic ( std::string plaintext, std::string publicKey ) {
			return encryptPublic( plaintext, Keypair( publicKey, "" ) );
		}
		
		/**
		 * Decrypt text encrypted by @code encryptPrivate @endcode
		 *     Encrypted by one
		 * --> Decrypted by any
		 * @param cyphertext Text
		 * @param publicKey key
		 * @return Plain text
		 */
		inline static std::string decryptPublic ( std::string cyphertext, std::string publicKey ) {
			return decryptPublic( cyphertext, Keypair( publicKey, "" ) );
		}
		
		/**
		 * Decrypted with @code decryptPublic @endcode
		 * --> Encrypted by one
		 *     Decrypted by any
		 * @param plaintext Text
		 * @param privateKey key
		 * @return Cypher text
		 */
		inline static std::string encryptPrivate ( std::string plaintext, std::string privateKey ) {
			return encryptPrivate( plaintext, Keypair( "", privateKey ) );
		}
		
		/**
		 * Decrypt text encrypted by @code encryptPublic @endcode
		 *     Encrypted by any
		 * --> Decrypted by one
		 * @param cyphertext Text
		 * @param privateKey key
		 * @return Plain text
		 */
		inline static std::string decryptPrivate ( std::string cyphertext, std::string privateKey ) {
			return decryptPrivate( cyphertext, Keypair( "", privateKey ) );
		}
	};
}
