#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{

    using Common;
    using Photon.SocketServer;
    using SocketServer.Rpc;

    public class AnwserTextRequest : BaseRequest
    {
        public AnwserTextRequest()
        {
        }

        public AnwserTextRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Gets or sets an array of group identifiers the actor whants to join.
        /// </summary>
        [DataMember(Code = Const.Data1, IsOptional = false)]
        public string AnwserText { get; set; }
    }

}
#endif