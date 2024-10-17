/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#ifdef PLATFORM_WINDOWS

#include "skillquest/network.hpp"
#include "skillquest/network/winsock/Controller.hpp"
#include "skillquest/network/winsock/connection/ServerConnection.hpp"
#include <future>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include "skillquest/network/winsock/connection/ClientConnection.hpp"
#include "keepalive.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::winsock::connection {
	ServerConnection::ServerConnection (
			network::winsock::NetworkController* controller,
			skillquest::network::Address address
	) : skillquest::network::connection::ServerConnection(
			controller,
			address
	), _listen( nullptr ){
		auto socket = ::socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );
		if ( socket == INVALID_SOCKET ) {
			sq::shared()->logger()->error( "Server credentials failed to listen:\n {0}", WSAGetLastError() );
			::closesocket( socket );
		}

		_listen = reinterpret_cast<void*>( socket );

		sockaddr_in host;

		host.sin_family = AF_INET;
		host.sin_port = htons( address.port() );
		inet_pton( AF_INET, address.ip().c_str(), &host.sin_addr );
		memset( &(host.sin_zero), '\0', 8 );
		
		if ( ::bind( socket, reinterpret_cast<const SOCKADDR*>(&host), sizeof( host ) ) == SOCKET_ERROR ) {
			sq::shared()->logger()->error( "Failed to bind socket to {0}", address.toString() );
			closesocket( socket );
			throw std::runtime_error( "Unable to bind socket to " + address.toString() );
		}

		// TODO: packet timeout
		int option = 1;
		setsockopt(( SOCKET )socket, SOL_SOCKET, SO_REUSEADDR, reinterpret_cast< char* >( &option ), sizeof(option));

		if ( listen( socket, SOMAXCONN ) == -1 ) {
			sq::shared()->logger()->error( "Failed to listen on inet socket {0}:\n{1}", address.toString(), WSAGetLastError() );
			closesocket( socket );
			throw std::runtime_error( "Unable to listen on inet socket " + address.toString() );
		}
		
		_accepting = std::thread(
				[ this ] () {
					while ( _listen != nullptr ) {
						accept();
					}
				}
		);
		_accepting.detach();
	}
	
	ServerConnection::~ServerConnection () {
		closesocket( reinterpret_cast<SOCKET>( _listen ) );
		_listen = nullptr;
	}
	
	void ServerConnection::accept () {
		if ( _listen == nullptr ) return;
		sockaddr_in addr;
		socklen_t size = sizeof( addr );
		
		char host[NI_MAXHOST];
		char remote[NI_MAXSERV];
		
		SOCKET socket = ::accept( reinterpret_cast<SOCKET>( _listen ) , reinterpret_cast<sockaddr*>(&addr), &size );
		auto socket_ptr =  reinterpret_cast<void*>( socket );

		if ( socket == INVALID_SOCKET ) {
			sq::shared()->logger()->error( "Failed to accept socket" );
			throw std::runtime_error( "Unable to accept inet socket" );
		}
		auto res = ::getnameinfo(
				( sockaddr* ) &addr, size,
				host, NI_MAXHOST,
				remote, NI_MAXSERV, 0
		);
		if ( res == -1 ) {
			sq::shared()->logger()->error( "Unable to get socket address" );
			return;
		}
		
		winsock::set_tcp_keepalive_cfg( (void*) socket, { 60, 6, 10 } );
		
		auto internalAddress = skillquest::network::Address( std::string( inet_ntoa( addr.sin_addr ) ),
															ntohs( addr.sin_port ) );
		auto client = std::shared_ptr< skillquest::network::connection::ClientConnection >(
				new network::winsock::connection::ClientConnection(
						dynamic_cast<network::winsock::NetworkController*>(_controller),
						internalAddress,
						socket_ptr
				),
				[ this ] ( skillquest::network::connection::ClientConnection* ptr ) {
					auto i = _clients.find( ptr->address() );
					if ( i != _clients.end() ) {
						sq::shared()->logger()->trace( "Erasing credentials @ {0}", ptr->address() );
						_clients.erase( i );
					}
					delete ptr;
				}
		);
		
		_clients[ internalAddress ] = client;
		
		controller()->channels().create( "", false )->send(
				client,
				new skillquest::network::packet::CryptoKeyToClientPacket(
						client->localKey().publicKey()
				)
		);
		
		connections()[ client->address() ] = client;
		controller()->clients().connection( client );
	}
	
	
	void ServerConnection::disconnect () {
		_clients.clear();
		closesocket( reinterpret_cast<SOCKET>( _listen ) );
		_listen = nullptr;
	}
}

#endif