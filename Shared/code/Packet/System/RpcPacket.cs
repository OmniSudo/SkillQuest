using System.Text.Json.Nodes;

namespace SkillQuest.Packet.System;

public class RpcPacket : Network.Packet {
    public string TypeName { get; set; }
    public string MethodName { get; set; }
    public JsonArray Arguments { get; set; }
}