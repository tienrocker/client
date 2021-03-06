﻿#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using GuestSong;
    using UnityEngine;
    public class AnwserTextResponse
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int SubCode { get { return MessageTag.G_ANWSER_TEXT; } }

        public AnwserTextResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new Exception("AnwserTextResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("AnwserTextResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new Exception("AnwserTextResponse: Invalid Data 1 - Text");

            try
            {
                this.Id = (int)operationResponse.Parameters[Const.Data1];
                this.Text = operationResponse.Parameters[Const.Data2] as string;

                if (NetworkManager.responseDebug) Debug.LogFormat("Player buzz: {0} with awnser {1}", this.Id, this.Text);
            }
            catch
            {
                throw new Exception("AnwserTextResponse: Invalid Data Type");
            }
        }
    }
}
#endif