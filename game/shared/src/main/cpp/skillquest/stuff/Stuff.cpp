/**
 * @author omnisudo
 * @date 2024.03.11
 */

#include "skillquest/stuff/Stuff.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::stuff {
	Stuff::~Stuff () {
	
	}
	
	bool Stuff::contains ( const URI& uri ) {
		return _things_by_scheme.contains( uri.scheme() ) && _things_by_scheme[ uri.scheme() ].contains( uri );
	}
	
	std::shared_ptr< IThing >& Stuff::operator[] ( const URI& uri ) {
		return _things_by_scheme[ uri.scheme() ][ uri ];
	}
	
	auto Stuff::things () -> std::vector< std::weak_ptr< IThing > > {
		std::vector< std::weak_ptr< IThing>> ret;
		
		for ( auto scheme: _things_by_scheme ) {
			for ( auto thing: scheme.second ) {
				ret.push_back( thing.second );
			}
		}
		
		return ret;
	}
	
	std::map< URI, std::weak_ptr< IThing > > Stuff::index ( const URI& search_partial ) {
		std::map< URI, std::weak_ptr< IThing >> result;
		
		for ( auto pair: _things_by_scheme[ search_partial.scheme() ] ) {
			if ( pair.first.authority() != search_partial.authority() ) continue;
			if ( pair.first.path().rfind( search_partial.path(), 0 ) != 0 ) continue;
			
			result.emplace( pair );
		}
		
		return result;
	}
	
	std::map< URI, std::weak_ptr< IThing>> Stuff::scheme ( const std::string& scheme ) {
		std::map< URI, std::weak_ptr< IThing >> result;
		
		for ( auto pair: _things_by_scheme[ scheme ] ) {
			result.emplace( pair );
		}
		
		return result;
	}
	
	bool Stuff::remove ( const URI& uri ) {
		if ( _things_by_scheme.contains( uri.scheme() ) ) {
			auto& things = _things_by_scheme[ uri.scheme() ];
			auto i = things.find( uri );
			if ( i != things.end() ) {
                i->second->deactivate();
				things.erase( i );
				if ( !_things_by_scheme.contains( uri.scheme() ) ) _things_by_scheme.erase( uri.scheme() );
				return true;
			}
		}
		return false;
	}
	
	bool Stuff::remove ( std::shared_ptr< IThing > thing ) {
		if ( !thing ) return false;

        auto uri = thing->uri();
        if ( _things_by_scheme.contains( uri.scheme() ) ) {
            auto& things = _things_by_scheme[ uri.scheme() ];
            auto i = things.find( uri );
            if ( i != things.end() && i->second.get() == thing.get() ) {
                i->second->deactivate();
                things.erase( i );
                if ( !_things_by_scheme.contains( uri.scheme() ) ) _things_by_scheme.erase( uri.scheme() );
                return true;
            }
        }
        return false;
	}
	
	std::map< URI, std::weak_ptr< IComponent>> Stuff::with ( const std::type_index& type ) {
		auto ret = std::map< URI, std::weak_ptr< IComponent > >{};
		
		for ( auto& scheme_pair: _things_by_scheme ) {
			for ( auto& thing_pair: scheme_pair.second ) {
				if ( !thing_pair.second ) continue;
				auto comp = thing_pair.second->component( type );
				if ( comp.expired() ) continue;
				ret.emplace( thing_pair.first, comp );
			}
		}
		
		return ret;
	}
	
	std::shared_ptr< IThing > Stuff::add ( IThing* thing ) {
		auto t = std::shared_ptr< IThing >{
				thing,
				[] ( const auto* ptr ) {
					delete ptr; // TODO: Why is this called on function exit?
				}
		};
		return add( t );
	}
	
	std::shared_ptr< IThing > Stuff::add ( std::shared_ptr< IThing > thing ) {
		if ( !thing ) return nullptr;
		if ( !_things_by_scheme.contains( thing->uri().scheme() ) ) {
			_things_by_scheme[ thing->uri().scheme() ] = {};
		}
		if ( _things_by_scheme[ thing->uri().scheme() ].contains( thing->uri() ) ) {
			_things_by_scheme[ thing->uri().scheme() ].erase( thing->uri() );
		}
		_things_by_scheme[ thing->uri().scheme() ][ thing->uri() ] = thing;
        thing->ptr_to_self( thing );
		sq::shared()->logger()->trace( "Added {0}", thing->uri().toString() );
		return thing;
	}
}