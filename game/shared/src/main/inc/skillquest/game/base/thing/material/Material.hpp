/**
 * @author omnisudo
 * @date 2024.07.31
 */

#pragma once

#include "skillquest/stuff.multiextendable.hpp"
#include "skillquest/string.hpp"
#include "skillquest/property.hpp"

namespace skillquest::game::base::thing::material {
	class Material : public stuff::MultiExtendable< Material > {
	public:
		struct CreateInfo {
			const stuff::MultiExtendable< Material >::CreateInfo& extends;
			std::string name;
		};
		
		explicit Material ( const CreateInfo& info ) :
				stuff::MultiExtendable< Material >{ info.extends },
				_name( info.name ) {
		}
		
		~Material () override = default;
	
	protected:
	property( name, std::string, public_const, private )
	
	};
}