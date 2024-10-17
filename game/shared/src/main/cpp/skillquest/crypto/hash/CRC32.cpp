/**
 * @author omnisudo
 * @data 2023.08.03
 */

#include "skillquest/crypto/hash/CRC32.hpp"

namespace skillquest::crypto::hash {
	void CRC32::generate_table ( std::uint32_t (& table)[256] ) {
		std::uint32_t polynomial = 0xEDB88320;
		for ( std::uint32_t i = 0; i < 256; i++ ) {
			std::uint32_t c = i;
			for ( auto j = 0; j < 8; j++ ) {
				if ( c & 1 ) {
					c = polynomial ^ ( c >> 1 );
				} else {
					c >>= 1;
				}
			}
			table[ i ] = c;
		}
	}
	
	uint32_t CRC32::update ( std::uint32_t (& table)[256], std::uint32_t initial, const void* buf, std::size_t len ) {
		std::uint32_t c = initial ^ 0xFFFFFFFF;
		const auto* u = static_cast<const std::uint8_t*>(buf);
		for ( auto i = 0; i < len; ++i ) {
			c = table[ ( c ^ u[ i ] ) & 0xFF ] ^ ( c >> 8 );
		}
		return c ^ 0xFFFFFFFF;
	}
}
