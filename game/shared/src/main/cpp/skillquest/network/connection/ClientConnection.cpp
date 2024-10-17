#include "skillquest/network/connection/ClientConnection.hpp"

#include "skillquest/network/NetworkController.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::network::connection {
	ClientConnection::ClientConnection (
			network::NetworkController* controller,
			network::Address address
	) :
			connection::IConnection( controller, address ),
			_key{},
			_email{ "" },
			_passhash{ "" },
			_signup{ false },
			_authenticated{ false } {
	}
	
	ClientConnection::~ClientConnection () {
		if ( _process.joinable() ) _process.join();
	}
	
	void ClientConnection::wait () {
		std::unique_lock lock( _mutex );
		_connected.wait( lock, [ this ] () { return !connected() || ( connected() && authenticated() ); } );
	}
	
	void ClientConnection::authenticated ( bool value ) {
		_authenticated = value;
		_connected.notify_all();
	}
	
	void ClientConnection::listen () {
		_process = std::thread(
				[ this ] () {
                    sq::shared()->logger()->trace( "Listening to {0}", address().toString() );
					do {
						if ( connected() ) {
							try {
								if ( receive().empty() ) throw std::runtime_error{ "Invalid packet" };
							} catch ( std::exception& e ) {
								sq::shared()->logger()->error(
										"Had to drop connection {0}:{1} {2}",
										_address.ip(), _address.port(), e.what()
								);
								disconnect();
							}
						} else {
							// TODO: Disconnect after timeout
							break;
						}
					} while ( true );
				}
		);
	}

    auto ClientConnection::uid () -> util::UID {
        if ( controller()->servers().authenticator() ) {
            return controller()->servers().authenticator()->uid( this->email() );
        }
        return {};
    }
}