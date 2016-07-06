#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Responses
{
    using System.Collections.Generic;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Custom.Common;
    using Interfaces;

    public class SonglistResponse : BaseResponse, IOperations
    {
        public SonglistResponse()
        {
        }

        public SonglistResponse(IRpcProtocol protocol, IDictionary<byte, object> parameter) : base(protocol, parameter)
        {
        }

        [DataMember(Code = Const.Data1, IsOptional = false)]
        public int[] Id { get; set; }

        [DataMember(Code = Const.Data2, IsOptional = false)]
        public string[] Name { get; set; }

        [DataMember(Code = Const.Data3, IsOptional = false)]
        public string[] Slug { get; set; }

        [DataMember(Code = Const.Data4, IsOptional = false)]
        public int[] PlaylistId { get; set; }

        [DataMember(Code = Const.Data5, IsOptional = false)]
        public string[] Url { get; set; }

        [DataMember(Code = Const.Data6, IsOptional = false)]
        public string[] BundleName { get; set; }

        [DataMember(Code = Const.Data7, IsOptional = false)]
        public string[] AssetName { get; set; }

        [DataMember(Code = MessageTag.KINGPLAY_OPERATION_TAG, IsOptional = false)]
        public int SubCode { get { return MessageTag.G_SONGLIST; } }

    }
}
#endif