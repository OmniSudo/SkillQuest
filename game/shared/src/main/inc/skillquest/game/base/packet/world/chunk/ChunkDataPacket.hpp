/**
 * @author omnisudo
 * @date 2024.09.01
 */

#pragma once

#include "skillquest/item.hpp"
#include "skillquest/network.hpp"
#include "skillquest/glm.hash.hpp"
#include <glm/vec3.hpp>
#include "skillquest/game/base/thing/world/chunk/Chunk.hpp"
#include <map>
#include "skillquest/base64.hpp"
#include "skillquest/uri.hpp"

#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::packet::world::chunk {
    class ChunkDataPacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * @param item
		 */
        explicit ChunkDataPacket ( std::shared_ptr< thing::world::chunk::Chunk > chunk ) :
                IPacket_INIT, _pos{ chunk->pos() } {
            for ( int i = 0; i < _blocks.size(); i++ ) {
                auto block = chunk->blocks()[ i ];
                if( !block ) {
                    _blocks[ i ] = 0;
                } else                {
                    auto id = std::hash< std::string >()( block->uri().toString() );
                    if ( !_ids.contains( id ) ) {
                        _ids.emplace( id, block->uri() );
                    }
                    _blocks[ i ] = id;
                }
            }
        }

        explicit ChunkDataPacket ( const json& data ) :
                network::IPacket( data ) {
            auto b64 = data[ "blocks" ].get< std::string >();
            auto decoded = convert::base64::decode( b64 );
            auto cstr = decoded.c_str();
            for ( auto i = 0; i < _blocks.size(); i++ ) {
                _blocks[ i ] = ::be64toh( *(( std::size_t* ) decoded.c_str() + i) );
            }
            for ( auto& [ id, uri ] : data[ "ids" ].items() ) {
                _ids.emplace( ( std::size_t ) std::stoull( id ), URI{ uri.get< std::string >() } );
            }
        }

        json serialize () const override {
            json data = IPacket::serialize();
            auto formatted = _blocks;
            for ( int i = 0; i < _blocks.size(); i++ ) {
                formatted[ i ] = ::htobe64( _blocks[ i ] );
            }
            std::string converted = convert::base64::encode( std::string( ( char* ) &formatted[ 0 ], formatted.size() * sizeof( std::size_t ) ) );
            data[ "blocks" ] = converted;
            auto ids = json{};
            for ( auto& [ id, uri ] : _ids ) {
                ids[ std::to_string( id ) ] = uri.toString();
            }
            data[ "ids" ] = ids;
            return data;
        }

    property( pos, glm::u16vec3, public_const, public_ptr );
    property( blocks,
              std::array<
                      std::size_t COMMA
                      thing::world::chunk::Chunk::SIZE *
                      thing::world::chunk::Chunk::SIZE *
                      thing::world::chunk::Chunk::SIZE
              >,
              public_ref, public );
    property( ids, std::map< std::size_t COMMA URI >, public_ref, public );
    };
}