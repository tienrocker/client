#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Operations.Requests
{
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class ProfileRequest : BaseRequest
    {
        public ProfileRequest()
        {
        }

        public ProfileRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Gets or sets an array of group identifiers the actor whants to join.
        /// </summary>
        [DataMember(Code = Const.Data1, IsOptional = false)]
        public int UserId { get; set; }
    }
}
#endif