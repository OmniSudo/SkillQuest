/**
 * @author OmniSudo
 * @date 3/3/2025
 */

#pragma once
#include <list>
#include <memory>
#include <stdexcept>
#include <type_traits>

namespace skillquest::engine::core {
template <typename... T> class Event {
public:
  class Handler {
  public:
    virtual void fire(T... args) = 0;

    virtual bool operator==(Handler &rhs) = 0;

  protected:
    Handler() = default;

    virtual ~Handler() = default;

    friend class Event;
  };

private:
  class HandlerWithStaticMethod : public Handler {
  public:
    HandlerWithStaticMethod(void (*callback)(T... args)) { _method = callback; }

    void fire(T... args) override { _method(args...); }

    bool operator==(Handler &rhs) override {
      auto other = dynamic_cast<Event<T...>::HandlerWithStaticMethod *>(&rhs);
      return typeid(*this) == typeid(rhs) && other != nullptr &&
             _method == other->_method;
    }

  private:
    void (*_method)(T... args);
  };

  template <typename Self> class HandlerWithMemberMethod : public Handler {
  public:
    HandlerWithMemberMethod(void (Self::*callback)(T... args), Self *self) {
      if (!self)
        throw std::invalid_argument("self is null");
      _self = self;
      _method = callback;
    }

    void fire(T... args) override {
      if (!_self)
        throw std::invalid_argument("self is null");
      (_self->*_method)(args...);
    }

    bool operator==(Handler &rhs) override {
      auto other = dynamic_cast<HandlerWithMemberMethod<Self> *>(&rhs);
      return typeid(*this) == typeid(rhs) && other != nullptr &&
             _method == other->_method && _self == other->_self;
    }

  private:
    Self *_self;

    void (Self::*_method)(T... args);
  };

public:
  inline static std::shared_ptr<Handler> bind(void (*callback)(T... args)) {
    return std::dynamic_pointer_cast<Handler>(
        std::make_shared<HandlerWithStaticMethod>(callback));
  }

  template <typename Self>
  inline static std::shared_ptr<Handler> bind(void (Self::*callback)(T... args),
                                              Self *self) {
    return std::dynamic_pointer_cast<Handler>(
        std::make_shared<HandlerWithMemberMethod<Self>>(callback, self));
  }

public:
  Event() {}

  ~Event() { m_eventHandlers.clear(); }

  void operator()(T... args) {
    for (auto handler : m_eventHandlers) {
      handler->fire(args...);
    }
  }

  Event &operator+=(std::shared_ptr<Handler> add) {
    if (!add)
      return *this;

    m_eventHandlers.push_back(add);

    return *this;
  }

  Event &operator-=(std::shared_ptr<Handler> remove) {
    if (!remove) {
      return *this; // a null passed, so nothing to do
    }

    std::erase_if(m_eventHandlers,
                  [this, remove](std::shared_ptr<Handler> &handler) {
                    return handler->operator==(*remove);
                  });

    return *this;
  }

private:
  Event(const Event &);
  Event &operator=(const Event &);

protected:
  std::vector<std::shared_ptr<Handler>> m_eventHandlers;
};
} // namespace skillquest::engine::core
