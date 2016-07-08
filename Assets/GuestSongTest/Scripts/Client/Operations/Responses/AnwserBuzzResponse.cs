#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using GuestSong;
    using UnityEngine;
    public class AnwserBuzzResponse
    {
        public int Id { get; set; }
        public int Time { get; set; }
        public int SubCode { get { return MessageTag.G_ANWSER_BUZZ; } }

        public AnwserBuzzResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new Exception("AnwserBuzzResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("AnwserBuzzResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new Exception("AnwserBuzzResponse: Invalid Data 2 - Time");

            try
            {
                this.Id = (int)operationResponse.Parameters[Const.Data1];
                this.Time = (int)operationResponse.Parameters[Const.Data2];

                if (NetworkManager.responseDebug) Debug.LogFormat("Player buzz: {0} with time {1}", this.Id, this.Time);
            }
            catch
            {
                throw new Exception("AnwserBuzzResponse: Invalid Data Type");
            }
        }
    }
}
#endif