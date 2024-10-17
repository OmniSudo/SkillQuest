/**
 * @author omnisudo
 * @date 2023.10.17
 */

#pragma once

namespace skillquest::network::socket {
	
	struct KeepConfig {
		/** The time (in seconds) the connection needs to remain
		 * idle before TCP starts sending keepalive probes (TCP_KEEPIDLE socket option)
		 */
		int keepidle;
		/** The maximum number of keepalive probes TCP should
		 * send before dropping the connection. (TCP_KEEPCNT socket option)
		 */
		int keepcnt;
		
		/** The time (in seconds) between individual keepalive probes.
		 *  (TCP_KEEPINTVL socket option)
		 */
		int keepintvl;
	};

/**
* enable TCP keepalive on the socket
* @param fd file descriptor
* @return 0 on success -1 on failure
*/
	int set_tcp_keepalive ( int sockfd );

/** Set the keepalive options on the socket
* This also enables TCP keepalive on the socket
*
* @param fd file descriptor
* @param fd file descriptor
* @return 0 on success -1 on failure
*/
	int set_tcp_keepalive_cfg ( int sockfd, KeepConfig const& cfg );
}