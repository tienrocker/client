#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{
    using Common;
    using Photon.SocketServer;
    using SocketServer.Rpc;

    public class PlaylistRequest : BaseRequest
    {
        public PlaylistRequest()
        {
        }

        public PlaylistRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }
    }
}
#endif