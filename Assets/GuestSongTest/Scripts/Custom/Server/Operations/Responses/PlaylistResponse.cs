#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Responses
{
    using System.Collections.Generic;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Custom.Common;
    using Interfaces;

    public class PlaylistResponse : BaseResponse, IOperations
    {
        public PlaylistResponse()
        {
        }

        public PlaylistResponse(IRpcProtocol protocol, IDictionary<byte, object> parameter) : base(protocol, parameter)
        {
        }

        [DataMember(Code = Const.Data1, IsOptional = false)]
        public int[] Id { get; set; }

        [DataMember(Code = Const.Data2, IsOptional = false)]
        public string[] Name { get; set; }

        [DataMember(Code = Const.Data3, IsOptional = false)]
        public string[] Slug { get; set; }

        [DataMember(Code = Const.Data4, IsOptional = false)]
        public bool[] Enable { get; set; }

        [DataMember(Code = Const.Data5, IsOptional = false)]
        public int[] Start { get; set; }

        [DataMember(Code = Const.Data6, IsOptional = false)]
        public int[] End { get; set; }

        [DataMember(Code = Const.Data7, IsOptional = false)]
        public int[] Type { get; set; }

        [DataMember(Code = Const.Data8, IsOptional = false)]
        public int[] Price { get; set; }

        [DataMember(Code = MessageTag.KINGPLAY_OPERATION_TAG, IsOptional = false)]
        public int SubCode { get { return MessageTag.G_PLAYLIST; } }
    }
}
#endif