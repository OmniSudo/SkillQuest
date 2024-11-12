using SkillQuest.API.Network;
using SkillQuest.API.Thing.Character;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Thing.Character.Player;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character.Select;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Character;

using Doohickey = Shared.Game.ECS.Doohickey;
using static Shared.Game.State;

public class CharacterSelect : Doohickey{
    public override Uri? Uri { get; } = new Uri("cl://control.skill.quest/character/select");

    IClientConnection _connection;

    IChannel _channel { get; }

    TaskCompletionSource<IPlayerCharacter[]> _characters;

    TaskCompletionSource<IPlayerCharacter> _selected;

    public CharacterSelect(IClientConnection connection){
        _connection = connection;
        _channel = SH.Net.CreateChannel(Uri);
        _characters = new TaskCompletionSource<IPlayerCharacter[]>();

        _channel.Subscribe<CharacterSelectInfoPacket>(OnCharacterSelectInfoPacket);
        _channel.Subscribe<SelectCharacterResponsePacket>(OnSelectCharacterResponsePacket);

        Reset();
    }

    public void Reset(){
        _characters = new TaskCompletionSource<IPlayerCharacter[]>();
        _selected = new TaskCompletionSource<IPlayerCharacter>();
    }

    void OnCharacterSelectInfoPacket(IClientConnection connection, CharacterSelectInfoPacket packet){
        _characters.SetResult(
            packet.Characters?.Select(
                character => new CharacterSelectPlayer(
                    character.CharacterId,
                    character.Name,
                    character.World,
                    character.Uri,
                    _connection
                )
            ).ToArray<IPlayerCharacter>() ?? Array.Empty<IPlayerCharacter>()
        );
    }

    void OnSelectCharacterResponsePacket(IClientConnection connection, SelectCharacterResponsePacket packet){
        if (!_characters.Task.IsCompletedSuccessfully || _selected.Task.IsCompletedSuccessfully)
            return;

        if (packet.Selected is not null) {
            var character = Character(packet.Selected.CharacterId).Result;

            if (character is null) {
                _selected.SetCanceled(new CancellationToken(true));
                return;
            }

            _selected.SetResult(character);
        } else {
            _selected.SetCanceled(new CancellationToken(true));
        }
    }

    async Task< IPlayerCharacter? > Character(Guid character){
        var characters = await _characters.Task;
        return characters.FirstOrDefault( player => player.CharacterId == character);
    }

    public async Task<IPlayerCharacter[]> Characters(){
        _channel.Send(_connection, new CharacterSelectInfoRequestPacket());
        return await _characters.Task;
    }

    public async Task<IPlayerCharacter?> Select(IPlayerCharacter? character){
        if (_characters.Task.IsCompletedSuccessfully) {
            if (character is not null) {
                _channel.Send(_connection, new SelectCharacterRequestPacket() { Id = character.CharacterId });
            }

            return await _selected.Task;
        }

        return null;
    }
}
