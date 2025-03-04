#include "skillquest/engine/crypto/hash/SHA256.hpp"
#include "skillquest/engine/util/convert.hpp"

#include <openssl/sha.h>
#include <filesystem>
#include <fstream>
#include <sstream>

namespace skillquest::crypto::hash::SHA256 {
	/**
	 * Returns B64 encoded SHA256 hash of the string
	 * @param str The string to hash
	 * @return B64 hash
	 */
	std::string string ( const std::string& str ) {
		unsigned char h[SHA256_DIGEST_LENGTH];
		::SHA256( reinterpret_cast< const unsigned char* >( str.c_str() ), str.length(), h );
		return convert::base64::encode( { reinterpret_cast< char* >( h ), SHA256_DIGEST_LENGTH } );
	}
	
	/**
	 * Digest a file
	 * @param path File path
	 * @return B64 hash
	 */
	std::string file ( std::filesystem::path path ) {
		unsigned char h[SHA256_DIGEST_LENGTH];
		std::ifstream in( path, std::fstream::in | std::fstream::binary );
		std::ostringstream data;
		if ( in ) {
			data << in.rdbuf();
			in.close();
		}
		return convert::base64::encode( string( data.str() ) );
	}
}