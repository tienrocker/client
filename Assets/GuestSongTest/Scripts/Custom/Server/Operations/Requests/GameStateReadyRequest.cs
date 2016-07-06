#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{
    using Photon.SocketServer;

    public class GameStateReadyRequest : BaseRequest
    {
        public GameStateReadyRequest()
        {
        }

        public GameStateReadyRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }
    }
}
#endif