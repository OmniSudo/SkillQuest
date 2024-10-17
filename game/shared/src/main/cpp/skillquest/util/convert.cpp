#include "skillquest/util/convert.hpp"

#include <vector>
#include <sstream>
#include <iomanip>

namespace skillquest::convert {
	namespace base64 {
		std::string encode ( const std::string& in ) {
			std::string out;
			
			int val = 0, valb = -6;
			for ( unsigned char c: in ) {
				val = ( val << 8 ) + c;
				valb += 8;
				while ( valb >= 0 ) {
					out.push_back( "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"[ ( val >> valb ) &
																									   0x3F ] );
					valb -= 6;
				}
			}
			if ( valb > -6 )
				out.push_back(
						"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"[
								( ( val << 8 ) >> ( valb + 8 ) ) &
								0x3F ] );
			while ( out.size() % 4 ) out.push_back( '=' );
			return out;
		}
		
		std::string decode ( const std::string& in ) {
			std::string out;
			
			std::vector< int > T( 256, -1 );
			for ( int i = 0; i < 64; i++ )
				T[ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"[ i ] ] = i;
			
			int val = 0, valb = -8;
			for ( unsigned char c: in ) {
				if ( T[ c ] == -1 ) break;
				val = ( val << 6 ) + T[ c ];
				valb += 6;
				if ( valb >= 0 ) {
					out.push_back( char( ( val >> valb ) & 0xFF ) );
					valb -= 8;
				}
			}
			return out;
		}
	}
	
	namespace hex {
		std::string encode ( const std::string& in ) {
			std::stringstream out;
			
			for ( auto c: in ) {
				out << std::hex << ( int ) c;
			}
			
			return out.str();
		}
		
		std::string decode ( const std::string& in ) {
			std::stringstream inss( in );
			std::stringstream outss;
			
			int i;
			for ( auto c: in ) {
				inss >> std::hex >> i;
				outss << ( char ) i;
			}
			
			return outss.str();
		}
	}
}