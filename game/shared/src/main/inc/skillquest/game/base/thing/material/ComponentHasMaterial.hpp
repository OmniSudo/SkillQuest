/**
 * @author omnisudo
 * @date 7/31/24
 */
#pragma once

#include "skillquest/stuff.component.hpp"
#include "Material.hpp"

namespace skillquest::game::base::thing::material {
	class ComponentHasMaterial : public stuff::IComponent {
	public:
		ComponentHasMaterial ( std::shared_ptr <Material> material ) : _material{ material } {
		
		}
		
		property(material, std::shared_ptr< Material >,
	public, none);
	};
}
