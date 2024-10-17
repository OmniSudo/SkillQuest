/**
 * @author omnisudo
 * @date 2023.07.21
 */

#include "skillquest/network/Packet.hpp"
#include "skillquest/network/connection/ClientConnection.hpp"

namespace skillquest::network {
	json IPacket::serialize () const {
		auto data = json();
		data[ "type" ] = _type;
		return data;
	}
}