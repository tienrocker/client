#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{

    using Photon.SocketServer;

    public class AnwserBuzzRequest : BaseRequest
    {
        public AnwserBuzzRequest()
        {
        }

        public AnwserBuzzRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }
    }

}
#endif