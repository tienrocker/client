#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using GuestSong;
    using UnityEngine;
    public class AnwserOptionResponse
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int SubCode { get { return MessageTag.G_ANWSER_OPTION; } }

        public AnwserOptionResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new Exception("AnwserOptionResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("AnwserOptionResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new Exception("AnwserOptionResponse: Invalid Data 2 - Index");

            try
            {
                this.Id = (int)operationResponse.Parameters[Const.Data1];
                this.Index = (int)operationResponse.Parameters[Const.Data2];

                if (NetworkManager.responseDebug) Debug.LogFormat("Player selected option anwser: {0} with index {1}", this.Id, this.Index);
            }
            catch
            {
                throw new Exception("AnwserOptionResponse: Invalid Data Type");
            }
        }
    }
}
#endif