/**
 * @author omnisudo
 * @date 2023.10.17
 */

#include "skillquest/platform.hpp"
#include "keepalive.hpp"

#if defined( PLATFORM_LINUX ) || defined( PLATFORM_WEB )

#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/tcp.h>

namespace skillquest::network::socket {
	int set_tcp_keepalive ( int sockfd ) {
		int optval = 1;
		
		return setsockopt( sockfd, SOL_SOCKET, SO_KEEPALIVE, reinterpret_cast<const char*>(&optval), sizeof( optval ) );
	}
	
	int set_tcp_keepalive_cfg ( int sockfd, const KeepConfig& cfg ) {
		int rc;
		
		//first turn on keepalive
		rc = set_tcp_keepalive( sockfd );
		if ( rc != 0 ) {
			return rc;
		}
		
		//set the keepalive options
		rc = setsockopt(
				sockfd, IPPROTO_TCP, TCP_KEEPCNT, reinterpret_cast<const char*>(&cfg.keepcnt), sizeof cfg.keepcnt
		);
		if ( rc != 0 ) {
			return rc;
		}
		
		rc = setsockopt(
				sockfd, IPPROTO_TCP, TCP_KEEPIDLE, reinterpret_cast<const char*>(&cfg.keepidle), sizeof cfg.keepidle
		);
		if ( rc != 0 ) {
			return rc;
		}
		
		rc = setsockopt(
				sockfd, IPPROTO_TCP, TCP_KEEPINTVL, reinterpret_cast<const char*>(&cfg.keepintvl), sizeof cfg.keepintvl
		);
		if ( rc != 0 ) {
			return rc;
		}
		
		return 0;
	}
}

#endif