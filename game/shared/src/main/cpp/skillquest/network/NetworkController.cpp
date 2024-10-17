#include "skillquest/network/NetworkController.hpp"
#include "skillquest/base64.hpp"
#include "skillquest/crypto/hash/SHA256.hpp"
#include "skillquest/network/packet/handshake/ConnectedPacket.hpp"

#include "skillquest/sh.api.hpp"

namespace skillquest::network {
	NetworkController::NetworkController () :
			_packets( new network::database::PacketDatabase( this ) ),
			_channels( new network::database::ChannelDatabase( this ) ),
			_channel( channels().create( "handshake", false ) ) {
		packets().add< packet::handshake::ConnectedPacket >();
		packets().add< packet::handshake::PublicKeyPacket >();
		packets().add< packet::handshake::EncryptedCredentialsPacket >();
		packets().add< packet::handshake::EncryptedSignupCredentialsPacket >();
		packets().add< packet::handshake::AuthenticationStatusPacket >();
		_channel->add( this, &NetworkController::onNet_PublicKeyPacket );
		_channel->add( this, &NetworkController::onNet_EncryptedCredentialsPacket );
		_channel->add( this, &NetworkController::onNet_EncryptedSignupCredentialsPacket );
		_channel->add( this, &NetworkController::onNet_AuthenticationStatusPacket );
	}
	
	NetworkController::~NetworkController () {
		delete _packets;
		delete _channels;
	}
	
	/**
	 * Server -> Client
	 * @param connection
	 * @param data
	 */
	void NetworkController::onNet_PublicKeyPacket (
			Connection connection,
			std::shared_ptr< packet::handshake::PublicKeyPacket > data
	) {
		if ( connection == nullptr ) return;
		if ( connection->key().public_key().empty() ) {
			sq::shared()->logger()->trace( "Received public key from server" );
			connection->key( data->public_key() );
			auto ciphertext = convert::base64::encode(
					crypto::cipher::RSA::encryptPublic( connection->passhash(), connection->key() )
			);
			// TODO: Remove references to username
			if ( connection->signup() ) {
				_channel->send(
						connection,
						new packet::handshake::EncryptedSignupCredentialsPacket(
								connection->email(), ciphertext
						)
				);
				sq::shared()->logger()->trace( "Sent encrypted signup credentials to server." );
			} else {
				_channel->send(
						connection,
						new packet::handshake::EncryptedCredentialsPacket(
								connection->email(), ciphertext
						)
				);
				sq::shared()->logger()->trace( "Sent encrypted credentials to server." );
			}
		}
	}
	
	
	/**
	 * Client -> Server; Response to PublicKeyPacket
	 * @param connection
	 * @param data
	 */
	void NetworkController::onNet_EncryptedCredentialsPacket (
			Connection connection,
			std::shared_ptr< packet::handshake::EncryptedCredentialsPacket > data
	) {
		if ( connection == nullptr ) return;
		if ( ( connection->email().empty() || connection->passhash().empty() ) ) {
			try {
				connection->email( data->email() );
				
				if ( connection->hasValidEmail() ) {
					auto password_hash = crypto::cipher::RSA::decryptPrivate(
							convert::base64::decode( data->password_hash() ),
							connection->key()
					);
					
					connection->passhash( password_hash );
					
					bool authed = this->servers().authenticator()->authenticate( connection );
					
					if ( authed ) {
						auto session = this->servers().authenticator()->login( connection );
						if ( !session.value.empty() ) {
							connection->session( session );
							sq::shared()->logger()->trace( "User {0} authentication success", connection->email() );
							_channel->send(
									connection,
									new packet::handshake::AuthenticationStatusPacket( connection->connected() )
							);
							this->channels().onConnected( connection ); // Broadcast ConnectedPacket to all channels
							return;
						}
					}
				}
				sq::shared()->logger()->trace( "User {0} authentication failure.", connection->email() );
				_channel->send(
						connection,
						new packet::handshake::AuthenticationStatusPacket( false, "invalid email or password" )
				);
				connection->disconnect();
				connection->authenticated( false );
			} catch ( const std::exception& e ) {
				sq::shared()->logger()->trace(
						"{0}: Failed to decrypt user credentials using private key; Disconnecting \"{1}\"",
						connection->address().toString(), e.what()
				);
				_channel->send(
						connection,
						new packet::handshake::AuthenticationStatusPacket( false, "internal server error" )
				);
				connection->disconnect();
				connection->authenticated( false );
			}
		}
	}
	
	/**
	 * Server -> Client; Response to EncryptedCredentialsPacket
	 * @param connection
	 * @param data
	 */
	void NetworkController::onNet_AuthenticationStatusPacket (
			Connection connection,
			std::shared_ptr< packet::handshake::AuthenticationStatusPacket > data
	) {
		if ( connection == nullptr ) return;
		if ( data->status() ) { // Connected
			connection->authenticated( true );
			_channel->process( connection, new packet::handshake::ConnectedPacket{} );
			this->channels().onConnected( connection ); // Broadcast ConnectedPacket to all channels
		} else { // Failed to connect
			sq::shared()->logger()
					->trace( "User {0} authentication failure: {1}", connection->email(), data->reason() );
			connection->disconnect();
		}
	}
	
	void NetworkController::onNet_EncryptedSignupCredentialsPacket (
			skillquest::network::Connection connection,
			std::shared_ptr< packet::handshake::EncryptedSignupCredentialsPacket > data
	) {
		if ( connection == nullptr ) return;
		if ( ( connection->email().empty() || connection->passhash().empty() ) ) {
			try {
				connection->email( data->email() );
				
				if ( connection->hasValidEmail() ) {
					auto password_hash = crypto::cipher::RSA::decryptPrivate(
							convert::base64::decode( data->password_hash() ),
							connection->key()
					);
					
					connection->passhash( password_hash );
					
					bool created = this->servers()
							.authenticator()
							->create( connection );
					
					if ( created ) {
						sq::shared()->logger()->trace( "User {0} signup authentication success", connection->email() );
						_channel->send(
								connection, new packet::handshake::AuthenticationStatusPacket( connection->connected() )
						);
						connection->authenticated( true );
						_channel->process( connection, new packet::handshake::ConnectedPacket{} );
					} else {
						auto exists = !this->servers().authenticator()->uid( connection->email() ).value.empty();
						sq::shared()->logger()->trace( "User {0} failed to create account.", connection->email() );
						_channel->send(
								connection, new packet::handshake::AuthenticationStatusPacket(
										false, exists ? "email already taken" : "invalid password"
								)
						);
						connection->disconnect();
						connection->authenticated( false );
					}
				} else {
					sq::shared()->logger()->trace( "User {0} failed to create account.", connection->email() );
					_channel->send(
							connection, new packet::handshake::AuthenticationStatusPacket(
									false, "invalid email"
							)
					);
					connection->disconnect();
					connection->authenticated( false );
				}
			} catch ( const std::exception& e ) {
				sq::shared()->logger()->trace(
						"{0}: Failed to decrypt user credentials for signup using private key; Disconnecting \"{1}\"",
						connection->address().toString(), e.what()
				);
				_channel->send(
						connection,
						new packet::handshake::AuthenticationStatusPacket( false, "internal server error" )
				);
				connection->disconnect();
				connection->authenticated( false );
			}
		}
	}
}

