/**
 * @author  omnisudo
 * @date    2024.11.02
 */

#include "ClientCharacterInventoryNetworking.hpp"

#include "skillquest/game/base/doohickey/item/ClientItemStackNetworking.hpp"

namespace skillquest::game::base::doohickey::character {
    ClientCharacterInventoryNetworking::ClientCharacterInventoryNetworking(const CreateInfo &info)
        : stuff::Doohickey{
              {.uri = CL_URI}
          },
          _channel{
              sq::shared()->network()->channels().create("character", true)
          } {
        sq::shared()->network()->packets().add<packet::inventory::character::CharacterInventoryInfoPacket>();
        _channel->add(this, &ClientCharacterInventoryNetworking::onNet_CharacterInventoryInfoPacket);
    }

    ClientCharacterInventoryNetworking::~ClientCharacterInventoryNetworking() {
        sq::shared()->network()->channels().destroy(_channel);
    }

    void ClientCharacterInventoryNetworking::onNet_CharacterInventoryInfoPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::character::CharacterInventoryInfoPacket> data
    ) {
        auto character = stuff().contains( data->character() ) ? std::dynamic_pointer_cast< sq::sh::Character::element_type >(stuff()[ data->character() ] ) : nullptr;
        character->inventory();
    }
}
