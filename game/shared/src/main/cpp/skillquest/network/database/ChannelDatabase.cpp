#include "skillquest/network/database/ChannelDatabase.hpp"

#include "skillquest/network/NetworkController.hpp"

namespace skillquest::network::database {
	ChannelDatabase::ChannelDatabase ( network::NetworkController* controller ) : _controller( controller ) {
	}
	
	Channel*& ChannelDatabase::create ( std::string id, bool encrypt ) {
		if ( !_channels.count( id ) ) {
			_channels[ id ] = new Channel( _controller, id, encrypt );
		}
		return _channels[ id ];
	}
	
	Channel*& ChannelDatabase::get ( std::string id ) {
		auto i = _channels.find( id );
		if ( i == _channels.end() ) return create( id ); // TODO: Throw some exception
		return i->second;
	}
	
	void ChannelDatabase::destroy ( network::Channel* channel ) {
		auto i = _channels.find( channel->name() );
		if ( i == _channels.end() ) return;
		_channels.erase( i );
		delete channel;
	}
	
	void ChannelDatabase::onConnected ( network::Connection connection ) {
		for ( auto& channel: _channels ) {
			channel.second->process( connection, new packet::handshake::ConnectedPacket{} );
		}
	}
}