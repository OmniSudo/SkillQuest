/**
 * @author omnisudo
 * @date 2024.09.02
 */

#include "skillquest/block.hpp"
#include "skillquest/network.hpp"
#include "skillquest/uri.hpp"

namespace skillquest::game::base::packet::block {
    class BlockInfoDeniedPacket : public network::IPacket {
    public:
        /**
		 * SERVER -> CLIENT
		 * TODO: block properties to packet properties
		 * @param block
		 */
        explicit BlockInfoDeniedPacket ( const URI& block ) :
                IPacket_INIT,
                _uri{ block } {
        }

        explicit BlockInfoDeniedPacket ( const json& data ) :
                network::IPacket( data ),
                _uri{ data[ "uri" ].get< std::string >() } {
        }

        json serialize () const override {
            json data = IPacket::serialize();
            data[ "uri" ] = uri().toString();
            return data;
        }

    property( uri, URI, public_const, public_ptr );
    };
}