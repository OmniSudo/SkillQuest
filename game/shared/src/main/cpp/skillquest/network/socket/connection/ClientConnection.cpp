/**
 * @author omnisudo
 * @date 2023.07.20
 */

#include "skillquest/platform.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include "keepalive.hpp"
#include "skillquest/base64.hpp"
#include "skillquest/network/database/ChannelDatabase.hpp"
#include "skillquest/network/socket/Controller.hpp"
#include "skillquest/network/socket/connection/ClientConnection.hpp"
#include "skillquest/sh.api.hpp"
#include <arpa/inet.h>
#include <errno.h>
#include <netdb.h>
#include <netinet/tcp.h>
#include <sys/socket.h>
#include <sys/types.h>


namespace skillquest::network::socket::connection {
    ClientConnection::ClientConnection(
            network::socket::NetworkController* controller,
            skillquest::network::Address address ) : skillquest::network::connection::ClientConnection( controller, address ) {
        _socket = ::socket( AF_INET, SOCK_STREAM, 0 );
        if( _socket == -1 ) {
            sq::shared()->logger()->error( "Failed to open socket!" );
            throw std::runtime_error( "Unable to open a socket" );
        }
    }

    ClientConnection::ClientConnection(
            network::socket::NetworkController* controller,
            network::Address address,
            int socket ) : skillquest::network::connection::ClientConnection( controller,
                                                                              { "0.0.0.0", 0 } ),
                           _socket( socket ) {
        sockaddr_in addr{};
        auto length = sizeof( addr );
        auto res = getsockname( _socket, ( sockaddr* ) &addr, reinterpret_cast< socklen_t* >( &length ) );
        if( res == -1 ) {
            sq::shared()->logger()->error( "Unable to user socket address" );
            return;
        }
        _address = address;
        listen();
    }

    ClientConnection::~ClientConnection() {
    }

    auto ClientConnection::connect(
            const std::string& email, const std::string& password_hash, bool signup ) -> void {
        this->passhash( password_hash );
        this->email( email );
        this->signup( signup );

        struct addrinfo hints;
        struct addrinfo *result, *rp;
        int sfd, s;

        /* Obtain address(es) matching host/port */

        memset( &hints, 0, sizeof( struct addrinfo ) );
        hints.ai_family = AF_UNSPEC;     /* Allow IPv4 or IPv6 */
        hints.ai_socktype = SOCK_STREAM; /* Datagram socket */
        hints.ai_flags = 0;
        hints.ai_protocol = 0; /* Any protocol */

        s = getaddrinfo(
                address().ip().c_str(),
                util::toString( address().port() ).c_str(),
                &hints, &result );

        if( s != 0 ) {
            throw std::runtime_error( "No hosts found when connecting to " + this->address().toString() );
        }

        /* getaddrinfo() returns a list of address structures.
            Try each address until we successfully connect(2).
            If socket(2) (or connect(2)) fails, we (close the socket
            and) try the next address. */

        for( rp = result; rp != NULL; rp = rp->ai_next ) {
            sfd = ::socket( rp->ai_family, rp->ai_socktype,
                            rp->ai_protocol );
            if( sfd == -1 )
                continue;

            if( ::connect( sfd, rp->ai_addr, rp->ai_addrlen ) != -1 )
                break; /* Success */

            shutdown( _socket, SHUT_RDWR );
#if !defined( PLATFORM_WEB )
            close( sfd );
#endif
        }

        if( rp == NULL ) { /* No address succeeded */
            throw std::runtime_error( "No valid hosts found when connecting to " + this->address().toString() );
        }

        freeaddrinfo( result ); /* No longer needed */

        _socket = sfd;
        socket::set_tcp_keepalive_cfg( _socket, { 60, 6, 10 } );

        listen();
    }

    auto ClientConnection::connected() const -> bool {
        return _socket > 0;
    }

    void ClientConnection::disconnect() {
        if( controller()->servers().authenticator() ) {
            controller()->servers().authenticator()->logout( session() );
        }
        shutdown( _socket, SHUT_RDWR );
#ifndef PLATFORM_WEB
        close( _socket );
#endif
        _socket = -1;
    }

    auto ClientConnection::send(
            json packet ) -> json {
        std::scoped_lock lock( _sending );

        auto text = packet.dump();
        uint32_t length = htonl( text.length() );

        ::send( _socket, &length, sizeof( uint32_t ), MSG_NOSIGNAL );
        ::send( _socket, text.c_str(), text.size(), MSG_NOSIGNAL );

        return packet;
    }

    const uint32_t max_size = 1024000;

    auto ClientConnection::receive() -> ::json {
        std::scoped_lock lock( _receiving );

        uint32_t size;
        if( ::recv( _socket, &size, sizeof( uint32_t ), 0 ) < 0 ) {
            sq::shared()->logger()->error( "Socket stream read length error: {0}", size );
        }
        size = ntohl( size );

        if( size == 0 || size > max_size ) {
            sq::shared()->logger()->error( "Received invalid packet from {0}", address().toString() );
            return {};
        }


        std::vector< uint8_t > bytes;
        bytes.resize( size, 0x00 );

        auto rcvlen = ::recv( _socket, &( bytes[ 0 ] ), size, 0 );
        if( rcvlen < 0 ) {
            sq::shared()->logger()->error( "Socket stream read data error" );
        }
        auto text = std::string{ bytes.begin(), bytes.end() };
        auto json = json::parse( text );

        if( json.size() < 3 ) return {};
        auto name = json[ 0 ].get< std::string >();

        auto channel = _controller->channels().get( name );

        channel->process( _controller->clients().connection( this->address() ), json );

        bytes.resize( 0 );

        return json;
    }
}// namespace skillquest::network::socket::connection

#endif
