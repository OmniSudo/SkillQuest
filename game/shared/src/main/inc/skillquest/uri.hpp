/**
 * @author omnisudo
 * @date 2024.03.01
 */

#pragma once

#include <iostream>
#include "skillquest/string.hpp"
#include <sstream>
#include <stdexcept>
#include "skillquest/json.hpp"

namespace skillquest {
    class URI : public util::ToString {
    private:
        std::string _full;
        std::string _scheme;
        std::string _authority;
        std::string _path;
        std::string _query;
        std::string _fragment;

    public:
        URI ( const std::string& uriString ) {
            parse( uriString );
        }

        ~URI () override = default;

        auto operator <=> ( const URI& other ) const {
            return _full <=> other._full;
        };

        std::string scheme () const {
            return _scheme;
        }

        std::string authority () const {
            return _authority;
        }

        std::string path () const {
            return _path;
        }

        std::string query () const {
            return _query;
        }

        /**
         * Split a ? query into param:value pairs
         * [asdf,qwer,zxcv] denotes an array of 3 elements asdf qwer zxcv
         * @return
         */
        json split_query () const {
            json query_params;
            std::istringstream iss( _query );
            std::string fullparam;
            while ( std::getline( iss, fullparam, '&' ) ) {
                std::istringstream issvalue( fullparam );

                std::string param, value;

                std::getline( issvalue, param, '=');
                std::getline( issvalue, value );

                if ( value.starts_with( '[' ) && value.ends_with( ']' ) ) {
                    std::istringstream issarray( value.substr( 1, std::string::npos - 1 ) );
                    std::string element;
                    query_params[ param ] = {};
                    while ( std::getline( issarray, element, ',' ) ) {
                        query_params[param].push_back( element );
                    }
                } else {
                    query_params[param] = value;
                }
            }

            return query_params;
        }

        std::string fragment () const {
            return _fragment;
        }

        std::string toString () const override {
            return _full;
        }

    private:
        void parse ( std::string uriString ) {
            if ( uriString.empty() ) return;

            std::istringstream iss( uriString );
            char c;

            // Parse scheme
            std::getline( iss, _scheme, ':' );

            // Check for "//"
            if ( iss.get() != '/' || iss.get() != '/' ) {
                throw std::invalid_argument( "Invalid URI format: Authority part is missing." );
            }

            // Parse authority
            std::getline( iss, _authority, '/' );

            // Parse path
            std::getline( iss, _path, '?' );

            // Parse query
            std::getline( iss, _query, '#' );

            // Parse fragment
            std::getline( iss, _fragment, '\0' );

            _full = uriString;
        }
    };

    struct HasURI {
        virtual auto uri () const -> URI = 0;
    };
}