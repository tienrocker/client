﻿#if UNITY_5_3_OR_NEWER
using ExitGames.Client.Photon;
using GuestSong;
using Photon.LoadBalancing.Custom.Common;
using System;
using UnityEngine;

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
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("ReadyPlayersResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("ReadyPlayersResponse: Invalid Data 1 - Id");

            try
            {
                this.Id = operationResponse.Parameters[Const.Data1] as int[];
                if (NetworkManager.responseDebug) Debug.LogFormat("List player ready: {0} ", LitJson.JsonMapper.ToJson(this.Id));
            }
            catch
            {
                throw new Exception("ReadyPlayersResponse: Invalid Data Type");
            }
        }
    }
}
#endif