#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using Custom.Common.Quiz;
    using GuestSong;
    using UnityEngine;
    public class GameStateResponse
    {
        public SongGameState State { get; set; }
        public int SubCode { get { return MessageTag.G_STATE_CHANGE; } }

        public GameStateResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new Exception("GameStateResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("GameStateResponse: Invalid Data 1 - State");

            try
            {
                this.State = (SongGameState)operationResponse.Parameters[Const.Data1];
                if (NetworkManager.responseDebug) Debug.LogFormat("Game state change: {0} ", this.State.ToString());
            }
            catch
            {
                throw new Exception("GameStateResponse: Invalid Data Type");
            }
        }
    }
}
#endif