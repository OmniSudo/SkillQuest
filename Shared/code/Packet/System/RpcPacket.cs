using System;
using System.Text.Json.Nodes;

namespace SkillQuest.Packet.System;

public static class RpcPacket {
    public class Request : Network.Packet {
        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public string[] Arguments { get; set; }
        public Guid RequestId { get; set; }
    }
    
    /// <summary>
    /// TODO: Create task that gets completed when the method responds
    /// </summary>
    public class Response : Network.Packet {
        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public string Ret { get; set; }
        public Guid RequestId { get; set; }
    }
}