/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#pragma once
#include <list>
#include <stdexcept>
#include <type_traits>

namespace skillquest::engine::core {
    template<typename... T>
    class Event {
    public:
        class Handler {
        public:
            virtual void fire( T&... args ) = 0;

            virtual bool operator == ( Handler& rhs ) = 0;

        private:
            Handler ();

            friend class Event;
        };

    private:
        class HandlerWithStaticMethod : Handler {
        public:
            HandlerWithStaticMethod(void (* callback)(T&... args)) {
                _method = callback;
            }

            void fire(T&... args) {
                _method(args...);
            }

            bool operator == ( Handler& rhs ) override {
                auto other = dynamic_cast< Event< T... >::HandlerWithStaticMethod* >( &rhs );
                return typeid( *this ) == typeid( rhs ) && other != nullptr && _method == other->_method;
            }

        private:
            void (* _method)(T&... args);

        };

        template<typename Self>
        class HandlerWithMemberMethod : Handler {
        public:
            HandlerWithMemberMethod(void (Self::* callback)(T&... args), Self* self) {
                if (!self) throw std::invalid_argument("self is null");
                _self = self;
                _method = callback;
            }

            void fire(T&... args) {
                if ( !_self ) throw std::invalid_argument("self is null");
                (_self->*_method)(args...);
            }

            bool operator == ( Handler& rhs ) override {
                auto other = dynamic_cast< Event< T... >::HandlerWithMemberMethod<Self>* >( &rhs );
                return typeid( *this ) == typeid( rhs ) && other != nullptr && _method == other->_method && _self == other->_self;
            }

        private:
            Self* _self;

            void (Self::* _method)(T&... args);
        };


    public:
        inline static Handler* bind( void(* callback)(T&... args) ) {
            return new HandlerWithStaticMethod( callback );
        }

        template < typename Self >
        inline static Handler* bind( void(Self::* callback)(T&... args), Self self ) {
            return new HandlerWithMemberMethod( callback, self );
        }

    public:
        Event () {}

        ~Event() {
            for(std::list< EventHandlerImpl<T>* >::iterator iter= m_eventHandlers.begin(); iter != m_eventHandlers.end(); ++iter)
            {
                EventHandlerImpl<T>* pHandler= *iter;
                if(pHandler)
                {
                    delete pHandler;
                    pHandler= 0;  // just to be consistent
                }
            }
            m_eventHandlers.clear();
        }

        void operator ()(T&... args) {
            Handler( &Event::operator (), this );
        }

        EventBase<T>& operator += (EventHandlerImpl<T>* pHandlerToAdd)
		{
			// bellow is commented because we decided to let the user add the same handler multiple time and make it his responsibility to remove all those added
			//if( FindHandlerWithSameBinding(pHandlerToAdd) != m_eventHandlers.end())

			if(pHandlerToAdd)
			{

#ifndef SHARP_EVENT_NO_BOOST
				// we are going to modify the handlers list, so we need a write lock.
				WriteLock handlersWriteLock(m_handlersMutex);
#endif // SHARP_EVENT_NO_BOOST

				// the handler added bellow along with all handlers in the list will be called later when an event is raised
				m_eventHandlers.push_back(pHandlerToAdd);
			}

			return *this;
		}

		/**
		* you can use this to remove a handler you previously added.
		* @note : removing a handler that was already removed is harmless, as this call does nothing and simply return when it does not find the handler.
		*	@example myEvent -= EventHandler::Bind(&ThisClass::OnMessage, this);  // example of unbinding in destructor
		*/
		EventBase<T>& operator -= (EventHandlerImpl<T>* pHandlerToRemove)
		{
			if( ! pHandlerToRemove)
			{
				return *this;  // a null passed, so nothing to do
			}

#ifndef SHARP_EVENT_NO_BOOST
			// we start by searching the handlers list and modify it ONLY when the passed handler is found
			UpgradeableReadLock handlersReadLock(m_handlersMutex);  // acquire a read lock for the search and switch to a write lock later when the handler is found and is to be deleted.
#endif // SHARP_EVENT_NO_BOOST

			// search for a handler that has the same binding as the passed one
			// search linearly (no other way)
			for( std::list< EventHandlerImpl<T>* >::iterator iter= m_eventHandlers.begin(); iter != m_eventHandlers.end(); ++iter)
			{
				EventHandlerImpl<T>* pHandler= *iter;
				if( pHandlerToRemove->IsBindedToSameFunctionAs(pHandler))
				{
#ifndef SHARP_EVENT_NO_BOOST
					// found the handler, we need to get a write lock as we are going to modify the handlers list.
					UpgradedWriteLock hanldersWriteLock(handlersReadLock);  // this get a write lock without releasing the read lock already acquired.
#endif // SHARP_EVENT_NO_BOOST

					// erase the memory that was created by the Bind function
					// this memory is that of an EventHandler class and has nothing to do with the actual functions/class passed to it on Bind
					EventHandlerImpl<T>* pFoundHandler= *iter;
					if( pFoundHandler)
					{
						delete pFoundHandler;
						pFoundHandler= 0;
					}

					// remove it form the list (safe to do it here as we'll break the loop)
					m_eventHandlers.erase(iter);
					break;
				}
			}

			// also delete the passed handler as we don't need it anymore (by design, Event always owns the memory of the handlers passed to it)
			if( pHandlerToRemove)
			{
				delete pHandlerToRemove;
				pHandlerToRemove= 0;
			}

			return *this;
		}

    private:
    	Event(const Event&);  ///< private to disable copying
    	Event& operator=(const Event&); ///< private to disable copying

    protected:
    	std::list< EventHandlerImpl<T>* > m_eventHandlers;
    };
}
