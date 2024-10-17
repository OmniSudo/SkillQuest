#include "skillquest/crypto/cipher/RSA.hpp"
#include <utility>
#include <openssl/rsa.h>
#include <openssl/pem.h>
#include <openssl/err.h>
#include "skillquest/crypto/cipher/exception/RSAException.hpp"

#include <iostream>

void error () {
	char err[128];
	ERR_error_string( ERR_get_error(), err );
	throw skillquest::crypto::cipher::RSAException( std::string( err ) );
}

::RSA* publicKey ( const skillquest::crypto::cipher::Keypair& keypair ) {
	if ( !keypair.public_key().empty() ) {
		std::string key = "-----BEGIN RSA PUBLIC KEY-----\n" + keypair.public_key() + "\n-----END RSA PUBLIC KEY-----";
		BIO* bio = BIO_new( BIO_s_mem() );
		if ( !bio ) error();
		BIO_write( bio, key.c_str(), key.length() );
		return PEM_read_bio_RSAPublicKey( bio, nullptr, nullptr, nullptr );
	}
	
	return nullptr;
}

::RSA* privateKey ( const skillquest::crypto::cipher::Keypair& keypair ) {
	if ( !keypair.secret_key().empty() ) {
		std::string key =
				"-----BEGIN RSA PRIVATE KEY-----\n" + keypair.secret_key() + "\n-----END RSA PRIVATE KEY-----";
		BIO* bio = BIO_new( BIO_s_mem() );
		if ( !bio ) error();
		BIO_write( bio, key.c_str(), key.length() );
		return PEM_read_bio_RSAPrivateKey( bio, nullptr, nullptr, nullptr );
	}
	
	return nullptr;
}

namespace skillquest::crypto::cipher {
	Keypair::Keypair ( const std::string& publicKey, const std::string& privateKey ) :
			_publicKey( publicKey ),
                                                                                      _secretKey( privateKey ) {
	}
	
	Keypair RSA::generate () {
		::RSA* pair = RSA_new();
		auto bn = BN_new();
		BN_set_word( bn, 5 );
		if ( !RSA_generate_key_ex( pair, 2048, bn, nullptr ) ) {
			BN_free( bn );
			error();
		}
		BN_free( bn );
		
		BIO* pri = BIO_new( BIO_s_mem() );
		BIO* pub = BIO_new( BIO_s_mem() );
		
		PEM_write_bio_RSAPrivateKey( pri, pair, nullptr, nullptr, 0, nullptr, nullptr );
		PEM_write_bio_RSAPublicKey( pub, pair );
		
		std::size_t priLength = BIO_pending( pri );
		std::size_t pubLength = BIO_pending( pub );
		
		char priKey[priLength + 1];
		char pubKey[priLength + 1];
		
		BIO_read( pri, priKey, priLength );
		BIO_read( pub, pubKey, pubLength );
		
		priKey[ priLength ] = 0;
		pubKey[ pubLength ] = 0;
		
		BIO_free( pri );
		BIO_free( pub );
		
		return Keypair(
				std::string( pubKey, pubLength ).substr( 31 ).substr( 0, pubLength - 31 - 30 ),
				std::string( priKey, priLength ).substr( 32 ).substr( 0, priLength - 32 - 31 )
		);
	}
	
	//TODO: rsa is nullptr
	std::string RSA::encryptPublic ( std::string plaintext, Keypair keypair ) {
		plaintext.resize( 2048 / 8 - 16 );
		auto rsa = publicKey( keypair );
		char encrypt[RSA_size( rsa )];
		int encryptLength;
		encryptLength = RSA_public_encrypt(
				plaintext.length(),
				reinterpret_cast<const unsigned char*>(plaintext.c_str()),
				( unsigned char* ) encrypt, rsa, RSA_PKCS1_PADDING
		);
		RSA_free( rsa );
		if ( encryptLength == -1 ) error();
		return std::string( encrypt, encryptLength );
	}
	
	std::string RSA::decryptPublic ( std::string cyphertext, Keypair keypair ) {
		auto rsa = publicKey( keypair );
		char decrypt[256];
		int decryptLength;
		decryptLength = RSA_public_decrypt(
				cyphertext.length(),
				reinterpret_cast<const unsigned char*>(cyphertext.c_str()),
				( unsigned char* ) decrypt, rsa, RSA_PKCS1_PADDING
		);
		RSA_free( rsa );
		if ( decryptLength == -1 ) error();
		return std::string( decrypt, decryptLength );
	}
	
	std::string RSA::encryptPrivate ( std::string plaintext, Keypair keypair ) {
		plaintext.resize( 2048 / 8 - 16 );
		auto rsa = privateKey( keypair );
		char encrypt[RSA_size( rsa )];
		int encryptLength;
		encryptLength = RSA_private_encrypt(
				plaintext.length(),
				reinterpret_cast<const unsigned char*>(plaintext.c_str()),
				( unsigned char* ) encrypt, rsa, RSA_PKCS1_PADDING
		);
		RSA_free( rsa );
		if ( encryptLength == -1 ) error();
		return std::string( encrypt, encryptLength );
	}
	
	std::string RSA::decryptPrivate ( std::string cyphertext, Keypair keypair ) {
		auto rsa = privateKey( keypair );
		char decrypt[256];
		int decryptLength;
		decryptLength = RSA_private_decrypt(
				cyphertext.length(),
				reinterpret_cast<const unsigned char*>(cyphertext.c_str()),
				( unsigned char* ) decrypt, rsa, RSA_PKCS1_PADDING
		);
		RSA_free( rsa );
		if ( decryptLength == -1 ) error();
		return std::string( decrypt, decryptLength );
	}
}