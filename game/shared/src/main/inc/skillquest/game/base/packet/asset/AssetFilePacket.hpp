/**
 * @author omnisudo
 * @date 2024.08.07
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/string.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::asset {
    class AssetFilePacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * @param item
		 */
        explicit AssetFilePacket( std::string filename, std::string data_b64 )
            : IPacket_INIT,
              _filename{ filename },
              _data_b64{ data_b64 } {
        }

        explicit AssetFilePacket( const json& data )
            : network::IPacket( data ),
              _filename{ data[ "file" ].get< std::string >() },
              _data_b64{ data[ "data" ].get< std::string >() } {
        }

        json serialize() const override {
            json data = IPacket::serialize();
            data[ "file" ] = filename();
            data[ "data" ] = data_b64();
            return data;
        }

        property( filename, std::string, public_const, public_ptr );
        property( data_b64, std::string, public_const, public_ptr );
    };
}