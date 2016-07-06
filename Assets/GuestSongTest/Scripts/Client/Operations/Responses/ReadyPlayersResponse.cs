#if UNITY_5_3_OR_NEWER
using ExitGames.Client.Photon;
using Photon.LoadBalancing.Custom.Common;
using System;

namespace Photon.LoadBalancing.Client.Operations.Responses
{
    public class ReadyPlayersResponse
    {
        public int[] Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public int SubCode { get { return MessageTag.G_READY_LIST; } }

        public ReadyPlayersResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("ProfileResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("ProfileResponse: Invalid Data 1 - Id");

            try
            {
                this.Id = operationResponse.Parameters[Const.Data1] as int[];
            }
            catch
            {
                throw new Exception("ProfileResponse: Invalid Data Type");
            }
        }
    }
}
#endif