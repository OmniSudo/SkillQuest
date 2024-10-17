#pragma once

#include "IConnection.hpp"
#include "skillquest/json.hpp"
#include <shared_mutex>
#include <functional>
#include <condition_variable>
#include <future>
#include <typeindex>
#include <queue>
#include "Connection.hpp"
#include "skillquest/crypto/cipher/RSA.hpp"
#include "skillquest/uid.hpp"

namespace skillquest::network::connection {
	/**
	 *
	 * @author  OmniSudo
	 * @date    06.12.21
	 * @date    27.02.23
	 */
	class ClientConnection : public connection::IConnection {
		std::mutex _mutex;
		
		friend class skillquest::network::NetworkController;
	
	protected:
		std::condition_variable _connected;
	
	public:
		ClientConnection (
				network::NetworkController* controller,
				network::Address address
		);
		
		virtual ~ClientConnection () override;
	
	public:
		void wait ();
		
		template < typename Rep, typename Period >
		void wait_for ( std::chrono::duration< Rep, Period > time ) {
			std::unique_lock lock( _mutex );
			_connected.wait_for( lock, time,
								 [ this ] () { return !connected() || ( connected() && authenticated() ); } );
		}
		
		template < typename Clock, typename Duration >
		void wait_until ( std::chrono::time_point< Clock, Duration > duration ) {
			std::unique_lock lock( _mutex );
			_connected.wait_until( lock, duration,
								   [ this ] () { return !connected() || ( connected() && authenticated() ); } );
		}
		
		virtual auto connect (
				const std::string& email, const std::string& password_hash, bool signup
		) -> void = 0;
		
		void listen ();
		
		virtual bool connected () const = 0;
		
		virtual void disconnect () = 0;
	
	property( key, crypto::cipher::Keypair, public, public );
	property( email, std::string, public, protected );
	property( passhash, std::string, public, protected );
	property( signup, bool, public, protected );
	property( authenticated, bool, public, none );
	property( session, util::UID, public, protected );

    public:
        auto uid () -> util::UID;

		bool hasValidEmail () {
			const std::regex pattern( R"((\w+)(\.|_)?(\w*)@(\w+)(\.(\w+))+)" );
			
			return std::regex_match( email(), pattern );
		}
	
	protected:
		std::thread _process;
	
	
	private:
		void authenticated ( bool value );
	
	public:
		virtual auto send ( ::json packet ) -> ::json = 0;
		
		virtual auto receive () -> ::json = 0;
		
	};
}
