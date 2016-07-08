#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{

    using Common;
    using Photon.SocketServer;
    using SocketServer.Rpc;

    public class AnwserOptionRequest : BaseRequest
    {
        public AnwserOptionRequest()
        {
        }

        public AnwserOptionRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = Const.Data1, IsOptional = false)]
        public int Index { get; set; }
    }

}
#endif