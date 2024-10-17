#include "skillquest/network/Channel.hpp"
#include "skillquest/network/connection/Connection.hpp"
#include "skillquest/network/Packet.hpp"
#include "skillquest/network/NetworkController.hpp"

#include <utility>

#include "skillquest/base64.hpp"
#include "skillquest/random.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/random/String.hpp"
#include "skillquest/crypto.evp.hpp"

namespace skillquest::network {
	Channel::~Channel () {
		controller()->channels()._channels[ this->name() ] = nullptr;
		controller()->channels()._channels.erase( this->name() );
	}
	
	/**
	 * TODO: Thrown on user logout
	 * @param what
	 */
	void Channel::logError ( const std::string what ) {
		sq::shared()->logger()->error( "Failed to complete a packet's reception: {0}", what );
	}
	
	auto Channel::send ( network::Connection connection, IPacket* packet ) -> Packet {
		return send( std::move( connection ), Packet( packet ) );
	}
	
	auto Channel::send ( network::Connection connection, Packet packet ) -> Packet {
		return send( encrypted(), std::move( connection ), packet );
	}
	
	auto Channel::send (
			bool encrypted,
			network::Connection connection,
			Packet packet
	) -> Packet {
		auto json = ::json();
		json.emplace_back( this->name() );
        auto data = packet->serialize().dump( 0 );
		
		if ( encrypted ) {
            auto ciphertext = crypto::cipher::EVP::encrypt(
                    data, connection->passhash(), connection->passhash().substr( 0, 8 )
            );

            json.emplace_back( convert::base64::encode( ciphertext ) );
		} else {
			json.emplace_back( data );
		}

        json.emplace_back( encrypted );

		connection->send( json );
		return packet;
	}
	
	auto Channel::process ( Connection connection, json json ) -> void {
		if ( connection == nullptr ) return;
		try {
            if ( json.size() < 3 ) return; // TODO: Log invalid packet
            auto data = json[ 1 ].get< std::string >();
            if ( json[ 2 ].get< bool >() ) {
                data = crypto::cipher::EVP::decrypt(
                        convert::base64::decode( data ),
                        connection->passhash(),
                        connection->passhash().substr( 0, 8 )
                );
            }
            auto ss = std::istringstream( data );
            auto json_data = json::parse( ss );
            auto packet = _controller->packets().get( json_data );

			if ( packet == nullptr ) {
				sq::shared()->logger()->trace(
						"Packet \"{0}\" does not have a packet parser",
                        json_data.contains( "type" ) ? json_data[ "type" ].get< std::string >() : "unknown type"
				);
				return;
			}
			
			process( connection, packet );
		} catch ( const std::exception& e ) {
			sq::shared()->logger()->trace( "Channel {0} failed to process packet:\n\t{1}", name(), e.what() );
		}
	}
	
	auto Channel::process ( Connection connection, Packet::element_type* packet ) -> void {
		process( connection, std::shared_ptr< IPacket >( packet ) );
	}
	
	auto Channel::process ( network::Connection connection, network::Packet packet ) -> void {
		if ( packet == nullptr ) {
			sq::shared()->logger()->error(
					"Connection @ {0} to {1} received null packet",
					connection->address(), connection->address()
			);
			return;
		}
		
		auto pair = _callbacks.find(packet->type());
		if ( pair == _callbacks.end() ) return;
		
		std::vector< std::shared_ptr< PacketListener > > listeners = { pair->second.begin(), pair->second.end() };
		for ( auto listener: listeners ) {
			try {
				if ( packet->consumed() ) return;
                if ( !listener || listener->type.empty() ) continue;
				if ( !listener->filter( connection, packet ) ) continue;
				listener->callback( connection, packet );
			} catch ( std::exception& e ) {
				sq::shared()->logger()->error( "Failed to process packet: {0}", e.what() );
			}
		}
	}
}
