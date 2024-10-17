/**
 * @author omnisudo
 * @date 2024.03.11
 */

#pragma once

#include "IThing.hpp"
#include "Thing.hpp"
#include "IStuff.hpp"
#include <map>

namespace skillquest::stuff {
	class Stuff : public IStuff {
	public:
		explicit Stuff ( const CreateInfo& info ) : IStuff{ info } {}
		
		~Stuff () override;
		
		auto contains ( const URI& uri ) -> bool override;
		
		auto operator[] ( const URI& uri ) -> std::shared_ptr< IThing >& override;
		
		auto things () -> std::vector< std::weak_ptr< IThing > > override;
		
		auto index ( const URI& search_partial ) -> std::map< URI, std::weak_ptr< IThing>> override;
		
		auto scheme ( const std::string& scheme ) -> std::map< URI, std::weak_ptr< IThing>> override;
		
		auto with ( const std::type_index& type ) -> std::map< URI, std::weak_ptr< IComponent>> override;
		
		auto remove ( const URI& uri ) -> bool override;
		
		auto remove ( std::shared_ptr< IThing > thing ) -> bool override;
	
	public:
		auto add ( IThing* thing ) -> std::shared_ptr< IThing > override;
		
		auto add ( std::shared_ptr< IThing > thing ) -> std::shared_ptr< IThing > override;
	
	private:
		std::map< std::string, std::map< URI, std::shared_ptr< IThing > > > _things_by_scheme;
	};
}