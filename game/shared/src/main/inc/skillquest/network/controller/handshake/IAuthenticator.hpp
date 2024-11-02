/**
 * @author omnisudo
 * @date 2024.07.29
 */

#pragma once

#include "skillquest/string.hpp"
#include "skillquest/network/connection/Connection.hpp"
#include "skillquest/uid.hpp"

namespace skillquest::network::controller::handshake {
	class IAuthenticator {
	public:
		virtual bool authenticate ( network::Connection connection ) = 0;
		
		/**
		 * Creates a user session
		 * @param connection
		 * @return Session UID
		 */
		virtual util::UID login ( network::Connection connection ) = 0;
		
		/**
		 * Destroys a user session
		 * @param connection
		 * @return
		 */
		virtual bool logout ( const util::UID& session_id ) = 0;
		
		virtual bool create ( const std::string& email, const std::string& passhash ) = 0;

		virtual bool create ( network::Connection connection ) = 0;

	public:
		virtual util::UID uid ( const std::string& username ) const = 0;

        virtual auto uid ( const std::string& email, const util::UID& uid ) -> void = 0;

		virtual bool exists ( const std::string& username ) const = 0;
		
		virtual std::string email ( const util::UID& uid ) const = 0;
	
	protected:
		virtual std::string password_hash ( const util::UID& uid ) const = 0;
		
		virtual std::string salt ( const util::UID& uid ) const = 0;
		
		/**
		 * Get Session UID of users' UID
		 * @param uid User UID
		 * @return Session UID
		 */
		virtual util::UID session ( const util::UID& uid ) const = 0;
		
	};
}