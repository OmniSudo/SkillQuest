/**
 * @author  omnisudo
 * @date    2024.11.03
 */

#include "BaseAddonCL.hpp"

namespace skillquest::game::base::doohickey::addon {
    std::shared_ptr<BaseAddonCL> BaseAddonCL::instance() {
        return std::dynamic_pointer_cast<BaseAddonCL>(
            sq::shared()->stuff()->contains(CL_URI)
                ? (*sq::shared()->stuff())[CL_URI]
                : nullptr
        );
    }

    BaseAddonCL::BaseAddonCL(const CreateInfo &info)
        : Doohickey{{.uri = CL_URI}},
          _player{info.player} {
    }

    BaseAddonCL::~BaseAddonCL() {
    }

    void BaseAddonCL::onActivate() {
        Doohickey::onActivate();

        _items = stuff().create<item::ClientItemNetworking>({.player = this->player()}, true);
        _itemstacks = stuff().create<item::ClientItemStackNetworking>({}, true);
        _blocks = stuff().create<block::ClientBlockNetworking>({}, true);
        _characters = stuff().create<character::ClientCharacterSystem>({}, true);

        world()->download({0, 0, 0});
    }

    void BaseAddonCL::onDeactivate() {
        stuff().remove(_items);
        stuff().remove(_itemstacks);
        stuff().remove(_blocks);
        stuff().remove(_characters);

        Doohickey::onDeactivate();
    }

    std::shared_ptr<thing::world::ClientWorld> BaseAddonCL::world() {
        auto lp = std::dynamic_pointer_cast<
            thing::character::player::LocalPlayer
        >(player());

        if (lp) {
            auto world = std::dynamic_pointer_cast<
                thing::world::ClientWorld
            >(lp->world());
            return world;
        }

        return nullptr;
    }

    auto BaseAddonCL::world(std::shared_ptr<thing::world::ClientWorld> world) -> void {
        auto lp = std::dynamic_pointer_cast<
            thing::character::player::LocalPlayer
        >(player());
        lp->world(world);
    }
}
