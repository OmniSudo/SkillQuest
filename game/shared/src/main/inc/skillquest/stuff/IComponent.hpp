/**
 * @author omnisudo
 * @date 2024.03.13
 */

#pragma once

#include <memory>

namespace skillquest::stuff {
	class IThing;
	
	class IComponent {
	public:
		virtual ~IComponent () = default;
		
		virtual auto thing () -> std::shared_ptr< IThing > = 0;
		
		virtual auto connect ( std::shared_ptr< IThing > thing ) -> std::weak_ptr< IComponent > = 0;
	
	protected:
		virtual auto onConnect ( std::shared_ptr< stuff::IThing > thing ) -> void = 0;
		
	};
}