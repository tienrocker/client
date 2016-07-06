#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{

    using Custom.Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class BaseRequest : Operation
    {
        public BaseRequest()
        {
        }

        public BaseRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }

        [DataMember(Code = MessageTag.KINGPLAY_OPERATION_CODE, IsOptional = true)]
        public virtual int SubCode { get; set; }
    }

}
#endif