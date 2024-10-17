/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX )

#include "skillquest/network.hpp"
#include "skillquest/network/socket/Controller.hpp"
#include "skillquest/network/socket/connection/ServerConnection.hpp"
#include <future>
#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>
#include "keepalive.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/network/packet/handshake/PublicKeyPacket.hpp"

namespace skillquest::network::socket::connection {
	
	ServerConnection::ServerConnection (
			network::socket::NetworkController* controller,
			skillquest::network::Address address
	) : skillquest::network::connection::ServerConnection(
			controller,
			address
	) {
		_socket = ::socket( AF_INET, SOCK_STREAM, 0 );
		if ( _socket == -1 ) {
			sq::shared()->logger()->error( "Failed to open inet socket!" );
			throw std::runtime_error( "Unable to open an inet socket" );
		}
		
		sockaddr_in host;
		host.sin_family = AF_INET;
		host.sin_port = htons( address.port() );
		inet_pton( AF_INET, address.ip().c_str(), &host.sin_addr );
		
		if ( bind( _socket, reinterpret_cast<const sockaddr*>(&host), sizeof( host ) ) == -1 ) {
			sq::shared()->logger()->error( "Failed to bind socket to {0}", address.toString() );
			throw std::runtime_error( "Unable to bind socket to " + address.toString() );
		}
		
		socket::set_tcp_keepalive_cfg( _socket, { 60, 6, 10 } );
		int option = 1;
		setsockopt( _socket, SOL_SOCKET, SO_REUSEADDR, &option, sizeof( option ) );
		
		if ( listen( _socket, SOMAXCONN ) == -1 ) {
			sq::shared()->logger()->error( "Failed to listen on inet socket {0}", address.toString() );
			throw std::runtime_error( "Unable to listen on inet socket " + address.toString() );
		}
		
		_accepting = std::thread(
				[ this ] () {
					while ( _socket != -1 ) {
						accept();
					}
				}
		);
		_accepting.detach();
	}
	
	ServerConnection::~ServerConnection () {
		shutdown( _socket, SHUT_RDWR );
		close( _socket );
		_socket = -1;
	}
	
	void ServerConnection::accept () {
		if ( _socket == -1 ) return;
		sockaddr_in addr;
		socklen_t size = sizeof( addr );
		
		char host[NI_MAXHOST];
		char remote[NI_MAXSERV];
		
		int socket = ::accept( _socket, reinterpret_cast<sockaddr*>(&addr), &size );
		
		if ( socket == -1 ) {
			sq::shared()->logger()->error( "Failed to accept socket" );
			throw std::runtime_error( "Unable to accept inet socket" );
		}
		auto res = ::getnameinfo(
				( sockaddr* ) &addr, size,
				host, NI_MAXHOST,
				remote, NI_MAXSERV, 0
		);
		if ( res == -1 ) {
			sq::shared()->logger()->error( "Unable to create socket address" );
			return;
		}
		
		socket::set_tcp_keepalive_cfg( socket, { 60, 6, 10 } );
		int option = 1;
		setsockopt( socket, SOL_SOCKET, SO_REUSEADDR, &option, sizeof( option ) );
		setsockopt( socket, SOL_SOCKET, SO_REUSEPORT, &option, sizeof( option ) );
		
		
		auto internalAddress = skillquest::network::Address( std::string( inet_ntoa( addr.sin_addr ) ),
															 ntohs( addr.sin_port ) );
		auto client = std::shared_ptr< skillquest::network::connection::ClientConnection >(
				new network::socket::connection::ClientConnection(
						dynamic_cast<network::socket::NetworkController*>(_controller),
						internalAddress,
						socket
				),
				[ this ] ( skillquest::network::connection::ClientConnection* ptr ) {
					auto i = _clients.find( ptr->address() );
					if ( i != _clients.end() ) {
						if ( !i->second->session().value.empty() )
							controller()->servers().authenticator()->logout( i->second->session() );
						sq::shared()->logger()->trace( "Erasing credentials @ address:{0}", ptr->address() );
						_clients.erase( i );
					}
					delete ptr;
				}
		);
		
		_clients[ internalAddress ] = client;
		connections()[ client->address() ] = client;
		controller()->clients().connection( client );
		
		client->key( this->rsa_key() );
		controller()->channels()
				.get( "handshake" )
				->send( client, new packet::handshake::PublicKeyPacket{ rsa_key().public_key() } );
	}
	
	
	void ServerConnection::disconnect () {
		_clients.clear();
		shutdown( _socket, SHUT_RDWR );
		close( _socket );
		_socket = -1;
	}
}

#elif defined( PLATFORM_WEB )

#include "skillquest/network.hpp"
#include "skillquest/network/socket/Controller.hpp"
#include "skillquest/network/socket/connection/ServerConnection.hpp"
#include <future>
#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>
#include "keepalive.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::socket::connection {

	ServerConnection::ServerConnection (
			network::socket::NetworkController* controller,
			skillquest::network::Address address
	) : skillquest::network::connection::ServerConnection(
			controller,
			address
	) {
		_socket = ::socket( AF_INET, SOCK_STREAM, 0 );
		if ( _socket == -1 ) {
			sq::shared()->logger()->error( "Failed to open inet socket!" );
			throw std::runtime_error( "Unable to open an inet socket" );
		}

		sockaddr_in host;
		host.sin_family = AF_INET;
		host.sin_port = htons( address.port() );
		inet_pton( AF_INET, address.ip().c_str(), &host.sin_addr );

		if ( bind( _socket, reinterpret_cast<const sockaddr*>(&host), sizeof( host ) ) == -1 ) {
			sq::shared()->logger()->error( "Failed to bind socket to {0}", address.toString() );
			throw std::runtime_error( "Unable to bind socket to " + address.toString() );
		}

		socket::set_tcp_keepalive_cfg( _socket, { 60, 6, 10 } );
		int option = 1;
		setsockopt(_socket, SOL_SOCKET, SO_REUSEADDR, &option, sizeof(option));

		if ( listen( _socket, SOMAXCONN ) == -1 ) {
			sq::shared()->logger()->error( "Failed to listen on inet socket {0}", address.toString() );
			throw std::runtime_error( "Unable to listen on inet socket " + address.toString() );
		}

		_accepting = std::thread(
				[ this ] () {
					while ( _socket != -1 ) {
						accept();
					}
				}
		);
		_accepting.detach();
	}

	ServerConnection::~ServerConnection () {
		shutdown( _socket, SHUT_RDWR );
		_socket = -1;
	}

	void ServerConnection::accept () {
		if ( _socket == -1 ) return;
		sockaddr_in addr;
		socklen_t size = sizeof( addr );

		char host[NI_MAXHOST];
		char remote[NI_MAXSERV];

		int socket = ::accept( _socket, reinterpret_cast<sockaddr*>(&addr), &size );

		if ( socket == -1 ) {
			sq::shared()->logger()->error( "Failed to accept socket" );
			throw std::runtime_error( "Unable to accept inet socket" );
		}
		auto res = ::getnameinfo(
				( sockaddr* ) &addr, size,
				host, NI_MAXHOST,
				remote, NI_MAXSERV, 0
		);
		if ( res == -1 ) {
			sq::shared()->logger()->error( "Unable to create socket address" );
			return;
		}

		socket::set_tcp_keepalive_cfg( socket, { 60, 6, 10 } );
		int option = 1;
		setsockopt( socket, SOL_SOCKET, SO_REUSEADDR, &option, sizeof(option) );
		setsockopt( socket, SOL_SOCKET, SO_REUSEPORT, &option, sizeof(option) );


		auto internalAddress = skillquest::network::Address( std::string( inet_ntoa( addr.sin_addr ) ),
															 ntohs( addr.sin_port ) );
		auto client = std::shared_ptr< skillquest::network::connection::ClientConnection >(
				new network::socket::connection::ClientConnection(
						dynamic_cast<network::socket::NetworkController*>(_controller),
						internalAddress,
						socket
				),
				[ this ] ( skillquest::network::connection::ClientConnection* ptr ) {
					auto i = _clients.find( ptr->address() );
					if ( i != _clients.end() ) {
						sq::shared()->logger()->trace( "Erasing credentials @ address:{0} address:{1}", ptr->address(),
												   ptr->address() );
						_clients.erase( i );
					}
					delete ptr;
				}
		);

		_clients[ internalAddress ] = client;
		connections()[ client->address() ] = client;
		controller()->clients().connection( client );
	}


	void ServerConnection::disconnect () {
		_clients.clear();
		shutdown( _socket, SHUT_RDWR );
		_socket = -1;
	}
}

#endif