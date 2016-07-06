#if !UNITY_5_3_OR_NEWER

namespace Photon.LoadBalancing.Custom.Server.Operations.Responses
{
    using System.Collections.Generic;
    using SocketServer;
    using SocketServer.Rpc;
    using Common;

    public class BaseResponse : DataContract
    {
        public BaseResponse()
        {
        }

        public BaseResponse(IRpcProtocol protocol, IDictionary<byte, object> parameter) : base(protocol, parameter)
        {
        }

        public OperationResponse OperationResponse
        {
            get
            {
                return new OperationResponse(MessageTag.KINGPLAY_OPERATION_CODE, this);
            }
        }
    }
}
#endif