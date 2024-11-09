/**
 * @author  omnisudo
 * @date    2024.11.03
 */

#include "ClientCharacterSystem.hpp"

namespace skillquest::game::base::doohickey::character {
    ClientCharacterSystem::ClientCharacterSystem(const CreateInfo &info)
        : stuff::Doohickey{{.uri = CL_URI}} {
        _inventories = stuff().create<ClientCharacterInventoryNetworking>(
            ClientCharacterInventoryNetworking::CreateInfo{}
        );
    }

    ClientCharacterSystem::~ClientCharacterSystem() {
        stuff().remove(_inventories);
    }
}
