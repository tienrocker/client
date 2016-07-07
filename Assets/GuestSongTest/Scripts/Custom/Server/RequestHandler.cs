#if !UNITY_5_3_OR_NEWER
using ExitGames.Logging;
using Photon.LoadBalancing.Custom.Common;
using Photon.LoadBalancing.GameServer;
using Photon.LoadBalancing.Master.OperationHandler;
using Photon.LoadBalancing.MasterServer;
using Photon.SocketServer;

namespace Photon.LoadBalancing.Custom.Server
{
    public class RequestHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Photon.LoadBalancing.Master.OperationHandler.OperationHandlerDefault.OperationResponse
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <returns></returns>
        public static OperationResponse TryPaser(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            int opCode = 0;
            object tagCode;
            if (operationRequest.Parameters.TryGetValue(MessageTag.KINGPLAY_OPERATION_TAG, out tagCode)) { opCode = int.Parse(tagCode.ToString()); }
            switch (opCode)
            {

                case MessageTag.U_PROFILE:
                    return ResponseHandler.ProfileResponse(peer, operationRequest, sendParameters);

                case MessageTag.G_PLAYLIST:
                    return ResponseHandler.PlaylistResponse(peer, operationRequest, sendParameters);

                default:
                    return OperationHandlerDefault.HandleUnknownOperationCode(operationRequest, log);
            }
        }

    }
}
#endif