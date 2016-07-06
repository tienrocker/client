#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Responses
{
    using System.Collections.Generic;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Custom.Common;
    using Interfaces;

    public class GameStateResponse : BaseResponse, IOperations
    {
        public GameStateResponse()
        {
        }

        public GameStateResponse(IRpcProtocol protocol, IDictionary<byte, object> parameter) : base(protocol, parameter)
        {
        }

        [DataMember(Code = Const.Data1, IsOptional = false)]
        public Common.Quiz.SongGameState State { get; set; }

        [DataMember(Code = MessageTag.KINGPLAY_OPERATION_TAG, IsOptional = false)]
        public int SubCode { get { return MessageTag.G_STATE_CHANGE; } }

    }
}
#endif