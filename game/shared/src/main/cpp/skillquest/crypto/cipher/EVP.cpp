#include "skillquest/crypto/cipher/EVP.hpp"

#include <openssl/evp.h>
#include <openssl/err.h>
#include "skillquest/crypto/cipher/exception/EVPException.hpp"
#include <sstream>

void error ( EVP_CIPHER_CTX* context ) {
	if ( context ) EVP_CIPHER_CTX_cleanup( context );
	throw skillquest::crypto::cipher::EVPException( ERR_reason_error_string( ERR_get_error() ) );
}

std::string crypt ( bool encrypt, std::string in, std::string key, std::string iv ) {
	key.resize( 32 );
	iv.resize( 16 );
	
	auto keyChars = reinterpret_cast< const unsigned char* >( key.c_str() );
	auto ivChars = reinterpret_cast< const unsigned char* >( iv.c_str() );
	
	unsigned char outBuffer[64];
	
	EVP_CIPHER_CTX* context = EVP_CIPHER_CTX_new();
	if ( !context ) error( context );
	
	if ( !EVP_CipherInit_ex( context, EVP_aes_256_cbc(), NULL, keyChars, ivChars, encrypt ) ) error( context );
	
	int oLength;
	int length = in.length();
	std::istringstream istream( in );
	std::ostringstream ostream;
	const int bs = 16;
	for ( ;; ) {
		int size = length >= bs ? bs : length % bs;
		std::string inBuf;
		inBuf.resize( size );
		istream.read( &inBuf[ 0 ], size );
		length -= bs;
		auto inChars = reinterpret_cast<const unsigned char*>(inBuf.c_str());
		if ( !EVP_CipherUpdate( context, outBuffer, &oLength, inChars, inBuf.length() ) )
			error( context );
		ostream << std::string( reinterpret_cast<const char*>(outBuffer), oLength );
		if ( length <= 0 ) break;
	}
	
	if ( !EVP_CipherFinal_ex( context, outBuffer, &oLength ) ) error( context );
	ostream << std::string( reinterpret_cast<const char*>(outBuffer), oLength );
	EVP_CIPHER_CTX_free( context );
	
	return ostream.str();
}

namespace skillquest::crypto::cipher {
	std::string EVP::encrypt ( std::string plaintext, std::string key, std::string iv ) {
		return crypt( true, plaintext, key, iv );
	}
	
	std::string EVP::decrypt ( std::string cyphertext, std::string key, std::string iv ) {
		return crypt( false, cyphertext, key, iv );
	}
}