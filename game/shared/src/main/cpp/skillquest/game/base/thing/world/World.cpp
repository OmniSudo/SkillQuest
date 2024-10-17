/**
 * @author omnisudo
 * @date 2024.08.01
 */

#include "skillquest/game/base/thing/world/World.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::thing::world {
    World::World ( const World::CreateInfo& info ) :
            stuff::Doohickey{ { .uri = world_uri( info.name ) } },
            _name{ info.name },
            _channel{ info.channel } ,
            _seed{ std::hash<std::string>{}( info.name ) }{
    }

    World::~World () {
        _channel->drop( this );
    }

    std::shared_ptr< character::PlayerCharacter > World::add_player (
            std::shared_ptr< character::PlayerCharacter > player
    ) {
        if ( !player ) {
            sq::shared()->logger()->error( "Tried to add null player to world {0}", uri() );
            return nullptr;
        }
        auto i = _players_by_name.find( player->name() );
        if ( i == _players_by_name.end() ) { // Not in map
            _players_by_name[ player->name() ] = player;
            return player;
        }
        return nullptr;
    }

    bool World::remove_player (
            std::shared_ptr< character::PlayerCharacter > player
    ) {
        if ( !player ) return false;
        auto i = _players_by_name.find( player->name() );
        if ( i != _players_by_name.end() ) { // Not in map
            _players_by_name.erase( i );
            stuff().remove( i->second );
            return true;
        }
        return false;
    }

    bool World::remove_player ( const util::UID& user_uid ) {
        auto i = _players_by_name.begin();
        while ( i != _players_by_name.end() ) {
            if ( i->second->uid() == user_uid ) {
                _players_by_name.erase( i++ );
                return true;
            } else {
                ++i;
            }
        }

        return false;
    }

    std::shared_ptr< world::chunk::Chunk > World::chunk ( glm::u16vec3 pos ) {
        auto i = _chunks.find( pos );
        if ( i == _chunks.end() ) return nullptr;
        return i->second;
    }

    std::shared_ptr< stuff::IThing > World::block ( glm::u64vec3 pos ) {
        auto chunk = this->chunk( { pos.x / 0x10, pos.y / 0x10, pos.z / 0x10 } );
        if ( chunk ) return chunk->get( { pos.x & 0xF, pos.y & 0xF, pos.z & 0xF } );
        return nullptr;
    }

    void World::block ( glm::u64vec3 pos, std::shared_ptr< stuff::IThing > block ) {
        auto chunk = this->chunk( { pos.x / 0x10, pos.y / 0x10, pos.z / 0x10 } );
        if ( chunk ) chunk->set( { pos.x & 0xF, pos.y & 0xF, pos.z & 0xF }, block );
    }
}