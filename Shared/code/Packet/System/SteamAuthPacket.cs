using Steamworks;

namespace SkillQuest.Packet.System;

public class SteamAuthPacket : Network.Packet {
    public byte[] Token { get; set; }

    public ulong SteamId { get; set; }
}