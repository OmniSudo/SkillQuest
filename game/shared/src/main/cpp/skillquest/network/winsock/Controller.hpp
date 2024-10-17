#pragma once

#include <map>
#include <memory>
#include "skillquest/network.hpp"

namespace skillquest::network::winsock {
	namespace database {
		class NetworkClientDatabase;
		
		class NetworkServerDatabase;
		
	}
	/**
	 * @author  OmniSudo
	 * @date    27.07.23
	 */
	class NetworkController : public skillquest::network::NetworkController {
		friend class database::NetworkClientDatabase;
		
		friend class database::NetworkServerDatabase;
		
		void* _wsaData = nullptr;
	
	public:
		explicit NetworkController ();
		
		~NetworkController () override;
	
	protected:
		auto WSADATA () const -> decltype( _wsaData ) { return _wsaData; }
		
	};
}