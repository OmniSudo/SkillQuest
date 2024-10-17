/**
 * @author omnisudo
 * @date 2024.03.12
 */

#include "skillquest/stuff/Thing.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/stuff/IStuff.hpp"
#include <deque>

namespace skillquest::stuff {
    Thing::Thing ( const IThing::CreateInfo& info ) :
            _stuff{ info.stuff },
            _uri{ info.uri },
            _active{ false } {

    }

    IStuff& Thing::stuff () {
        return _stuff ? *_stuff : *sq::shared()->stuff();
    }

    std::shared_ptr< IThing > Thing::parent () {
        return _parent.lock();
    }

    auto Thing::root () -> std::shared_ptr< IThing > {
        auto parent = this->parent();
        return parent ? parent->root() : self();
    }

    auto Thing::make_child_of ( std::weak_ptr< IThing > parent ) -> std::shared_ptr< IThing > {
        std::dynamic_pointer_cast< Thing >( parent.lock() )->_children
                                                           .emplace( this->uri(), stuff()[ this->uri() ] );
        this->_parent = parent;
        return self();
    }

    std::map< URI, std::weak_ptr< IThing>> Thing::children () {
        return { _children.begin(), _children.end() };
    }

    auto Thing::make_parent_of ( std::shared_ptr< IThing > child ) -> std::shared_ptr< IThing > {
        if ( !child ) return self();
        std::dynamic_pointer_cast< Thing >( child )->_parent = stuff()[ this->uri() ];
        this->_children.emplace( child->uri(), child );
        return self();
    }

    auto Thing::connect ( IComponent* component, const std::type_index& type ) -> std::weak_ptr< IComponent > {
        auto ptr = std::shared_ptr< IComponent >( component );

        _components[ type ] = ptr;
        if ( ptr && ptr->thing().get() != this ) ptr->connect( self() );

        return ptr;
    }

    auto Thing::component ( const std::type_index& type ) -> std::weak_ptr< IComponent > {
        if ( auto i = _components.find( type ); i != _components.end() ) {
            return i->second;
        }
        return std::shared_ptr< IComponent >{ nullptr };
    }

    std::map< URI, std::weak_ptr< IComponent>> Thing::with_component ( const std::type_index& type ) {
        std::deque< IThing* > things = { this };
        std::map< URI, std::weak_ptr< IComponent > > comps;
        while ( !things.empty() ) {
            auto thing = things.front();
            things.pop_front();

            if ( !thing ) continue;

            for ( auto& child: thing->children() ) {
                things.push_back( child.second.lock().get() );
            }

            auto comp = thing->component( type );

            if ( comp.expired() ) continue;
            auto c = comp.lock();
            if ( !c ) continue;

            comps[ thing->uri() ] = comp;
        }

        return comps;
    }

    auto Thing::disconnect ( const std::type_index& type ) -> std::shared_ptr< IComponent > {
        if ( auto i = _components.find( type ); i != _components.end() ) {
            auto p = i->second;
            _components.erase( i );
            return p;
        }
        return std::shared_ptr< IComponent >{ nullptr };
    }

    auto Thing::self () -> std::shared_ptr< IThing > {
        if ( !ptr_to_self().expired() ) return IThing::ptr_to_self().lock();
        return nullptr;
    }

    std::string Thing::toString () const {
        return uri().toString();
    }

    URI Thing::uri () const {
        return _uri;
    }

    bool Thing::active () {
        auto parent = _parent.lock();
        return ( !parent || parent->active() ) && _active;
    }

    void Thing::activate () {
        auto old = _active;
        _active = true;
        if ( !old ) {
            onActivate();
        }
    }

    void Thing::deactivate () {
        auto old = _active;
        _active = false;
        if ( old ) {
            onDeactivate();
        }
    }

    void Thing::onActivate () {
    }

    void Thing::onDeactivate () {
    }
}