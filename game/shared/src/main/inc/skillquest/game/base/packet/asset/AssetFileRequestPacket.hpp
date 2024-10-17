/**
* @author omnisudo
* @date 2024.08.07
*/

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/string.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::asset {
   class AssetFileRequestPacket : public network::IPacket {
   public:
       /**
        * CLIENT -> SERVER
        * Request to download a images' pixels. One image at a time
        * @param item
        */
       explicit AssetFileRequestPacket( std::string filename ) : IPacket_INIT,
                                                                                  _filename{ filename } {
       }

       explicit AssetFileRequestPacket( const json& data ) : network::IPacket( data ),
                                                        _filename{ data[ "file" ].get< std::string >() } {
       }

       json serialize() const override {
           json data = IPacket::serialize();
           data[ "file" ] = filename();
           return data;
       }

       property( filename, std::string, public_const, public_ptr );
   };
}// namespace skillquest::game::base::packet::item