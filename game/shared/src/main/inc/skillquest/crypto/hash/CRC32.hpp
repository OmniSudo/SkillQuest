/**
 * @author omnisudo
 * @data 2023.08.03
 */

#pragma once

#include <cstddef>
#include <cstdint>

namespace skillquest::crypto::hash {
	struct CRC32 {
		static void generate_table ( std::uint32_t(& table)[256] );
		
		static std::uint32_t
		update ( std::uint32_t (& table)[256], std::uint32_t initial, const void* buf, std::size_t len );
	};
}

// usage: the following code generates crc for 2 pieces of data
// uint32_t table[256];
// crc32::generate_table(table);
// uint32_t crc = crc32::update(table, 0, data_piece1, len1);
// crc = crc32::update(table, crc, data_piece2, len2);
// output(crc);