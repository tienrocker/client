#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using Custom.Models;
    using GuestSong;
    using UnityEngine;
    public class PlayListResponse
    {
        public ModelPlayList[] PlayLists { get; set; }
        public int SubCode { get { return MessageTag.G_PLAYLIST; } }

        public PlayListResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("PlayListResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("PlayListResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new Exception("PlayListResponse: Invalid Data 2 - Name");
            if (!operationResponse.Parameters.ContainsKey(Const.Data3)) throw new Exception("PlayListResponse: Invalid Data 3 - Slug");
            if (!operationResponse.Parameters.ContainsKey(Const.Data4)) throw new Exception("PlayListResponse: Invalid Data 4 - Enable");
            if (!operationResponse.Parameters.ContainsKey(Const.Data5)) throw new Exception("PlayListResponse: Invalid Data 5 - Start");
            if (!operationResponse.Parameters.ContainsKey(Const.Data6)) throw new Exception("PlayListResponse: Invalid Data 6 - End");
            if (!operationResponse.Parameters.ContainsKey(Const.Data7)) throw new Exception("PlayListResponse: Invalid Data 7 - Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data8)) throw new Exception("PlayListResponse: Invalid Data 8 - Price");

            try
            {

                int[] Id = operationResponse[Const.Data1] as int[];
                string[] Name = operationResponse[Const.Data2] as string[];
                string[] Slug = operationResponse[Const.Data3] as string[];
                bool[] Enable = operationResponse[Const.Data4] as bool[];
                int[] Start = operationResponse[Const.Data5] as int[];
                int[] End = operationResponse[Const.Data6] as int[];
                int[] Type = operationResponse[Const.Data7] as int[];
                int[] Price = operationResponse[Const.Data8] as int[];

                this.PlayLists = new ModelPlayList[Id.Length];
                for (int i = 0; i < Id.Length; i++)
                {
                    ModelPlayList info = new ModelPlayList();
                    info.id = Id[i];
                    info.name = Name[i];
                    info.slug = Slug[i];
                    info.enable = Enable[i];
                    info.start = Start[i];
                    info.end = End[i];
                    info.type = (ModelPlayList.PlaylistType)Type[i];
                    info.price = Price[i];

                    this.PlayLists[i] = info;
                }

                if (NetworkManager.responseDebug) Debug.LogFormat("PlayList : {0} ", LitJson.JsonMapper.ToJson(this.PlayLists));
            }
            catch
            {
                throw new Exception("PlayListResponse: Invalid Data Type");
            }
        }
    }
}
#endif