/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#ifdef PLATFORM_WINDOWS

#include "skillquest/network/winsock/connection/ClientConnection.hpp"
#include "skillquest/network/winsock/Controller.hpp"
#include "skillquest/random.hpp"
#include "skillquest/crypto.hpp"
#include "skillquest/base64.hpp"
#include "skillquest/api.hpp"
#include "skillquest/network/winsock/connection/keepalive.hpp"
#include "keepalive.hpp"
#include <errno.h>
#include <WS2tcpip.h>
#include "skillquest/sh.api.hpp"

namespace skillquest::network::winsock::connection {
	ClientConnection::ClientConnection (
			network::winsock::NetworkController* controller,
			skillquest::network::Address address
	) : skillquest::network::connection::ClientConnection( controller, address ) {
		_socket = reinterpret_cast<void*>(::socket( AF_INET, SOCK_STREAM, IPPROTO_TCP ));
		if ( reinterpret_cast<SOCKET>(_socket) == INVALID_SOCKET ) {
			sq::shared()->logger()->error( "Failed to open socket!" );
			throw std::runtime_error( "Unable to open a socket" );
		}
	}

	ClientConnection::ClientConnection (
			network::winsock::NetworkController* controller,
			network::Address address,
			void* socket
	) : skillquest::network::connection::ClientConnection(
			controller,
			address
	), _socket( socket ) {
		SOCKADDR_IN addr {};
		auto length = sizeof( addr );
		auto res = ::getsockname(
				reinterpret_cast<SOCKET>( _socket ),
				reinterpret_cast<sockaddr*>(&addr),
				reinterpret_cast<int*>(&length)
		);
		if ( res == -1 ) {
			sq::shared()->logger()->error( "Unable to get socket address" );
			return;
		}
		
		winsock::set_tcp_keepalive_cfg( socket, { 60, 6, 10 } );
		
		_process = std::async(
				std::launch::async,
				[ this ] () {
					while ( connected()) {
																	if ( receive().empty() ) throw std::runtime_error{ "Invalid packet" };

					}
				}
		);
	}

	ClientConnection::~ClientConnection () {
		disconnect();
	}

	auto ClientConnection::connect () -> void {
		sockaddr_in server;
		server.sin_family = AF_INET;
		server.sin_port = htons( this->address().port());
		inet_pton( AF_INET, this->address().ip().c_str(), &server.sin_addr );

		if ( ::connect( reinterpret_cast<SOCKET>( _socket ),
						reinterpret_cast<const sockaddr*>(&server),\
						sizeof( server )
						) == -1 ) {
			sq::shared()->logger()->error( "Failed to connect socket to {0}: {1}", this->address(), strerror(errno));
		}
		
		winsock::set_tcp_keepalive_cfg( socket, { 60, 6, 10 } );
		
		_process = std::async(
				std::launch::async,
				[ this ] () {
					sq::shared()->logger()->trace( "Connected to {0}", _address );

					controller()->channels().create( "", false )->send(
							std::shared_ptr< ClientConnection >( this, [] ( const auto* ptr ) {} ),
							new skillquest::network::packet::CryptoKeyToServerPacket( this->localKey().publicKey())
					);
																if ( receive().empty() ) throw std::runtime_error{ "Invalid packet" };

					_connected.notify_all();

					while ( connected()) {
																	if ( receive().empty() ) throw std::runtime_error{ "Invalid packet" };

					}
				}
		);
	}

	auto ClientConnection::connected () const -> bool {
		return _socket != nullptr;
	}

	void ClientConnection::disconnect () {
		_controller->channels().get( "" )->process(
				_controller->clients().connection( this->address() ), new packet::ConnectionShutdownPacket()
		);
		closesocket( reinterpret_cast<SOCKET>(_socket) );
		_socket = nullptr;
	}

	auto ClientConnection::send (
			json packet
	) -> json {
					std::scoped_lock lock( _sending );

					auto text = packet.dump();
					uint32_t length = htonl( text.size());

					::send( reinterpret_cast<SOCKET>(_socket), ( const char * ) &length, sizeof( length ), 0 );
					::send( reinterpret_cast<SOCKET>(_socket), text.c_str(), text.size(), 0 );

					return packet;
	}

	auto ClientConnection::receive () -> json {
					std::scoped_lock lock( _receiving );

					uint32_t size;
					::recv( reinterpret_cast<SOCKET>( _socket ), ( char* ) &size, sizeof( uint32_t ), 0 );
					size = ntohl( size );

					if ( size == 0 ) return {};

					std::vector< uint8_t > bytes;
					bytes.resize( size, 0x00 );

					::recv( reinterpret_cast<SOCKET>( _socket ), ( char* ) &( bytes[ 0 ] ), size, 0 );

					auto text = std::string { bytes.begin(), bytes.end() };
					auto json = json::parse( text );

					if ( json.size() < 2 ) return {};
					auto name = json[ 0 ].get< std::string >();

					auto channel = _controller->channels().get( name );

					channel->process( _controller->clients().connection( this->address()), json );
					return json;
	}
}

#endif
