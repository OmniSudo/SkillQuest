/**
 * @author omnisudo
 * @data 2023.07.29
 */

#pragma once

#define COMMA ,

#define GETTER( name ) auto name() -> decltype( _##name ){ return _##name; }
#define GETTER_CONST( name ) auto name() const -> decltype( _##name ) { return _##name; }
#define GETTER_REF( name ) auto name() -> decltype( _##name )& { return _##name; }
#define GETTER_none( name )
#define GETTER_public( name ) public: GETTER( name )
#define GETTER_protected( name ) protected: GETTER( name )
#define GETTER_private( name ) private: GETTER( name )
#define GETTER_public_ref( name ) public: GETTER_REF( name )
#define GETTER_protected_ref( name ) protected: GETTER_REF( name )
#define GETTER_private_ref( name ) private: GETTER_REF( name )
#define GETTER_public_const( name ) public: GETTER_CONST( name )
#define GETTER_protected_const( name ) protected: GETTER_CONST( name )
#define GETTER_private_const( name ) private: GETTER_REF( name )


#define SETTER( name ) auto name( const decltype( _##name )& value ) -> decltype( *this ) { this->_##name = value; return *this; }
#define SETTER_none( name )
#define SETTER_public( name ) public: SETTER( name )
#define SETTER_protected( name ) protected: SETTER( name )
#define SETTER_private( name ) private: SETTER( name )
#define SETTER_PTR( name ) auto name( const decltype( _##name )& value ) -> decltype( this ) { this->_##name = value; return this; }
#define SETTER_public_ptr( name ) public: SETTER_PTR( name )
#define SETTER_protected_ptr( name ) protected: SETTER_PTR( name )
#define SETTER_private_ptr( name ) private: SETTER_PTR( name )

/**
 * @param name 	The name of the property
 * @param type	The type of the variable
 * @param getter The visibility of the getter,
 * [none, public, protected, private, public_ref, protected_ref, private_ref]
 * @param setter The visibility of the setter,
 * [none, public, protected, private]
 */
#define property( name, type, getter, setter ) \
private: type _##name;                          \
GETTER_##getter(name)                         \
SETTER_##setter(name)
