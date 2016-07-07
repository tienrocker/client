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
        public int[] Id { get; set; }
        public string[] Name { get; set; }
        public string[] Slug { get; set; }
        public bool[] Enable { get; set; }
        public int[] Start { get; set; }
        public int[] End { get; set; }
        public int[] Type { get; set; }
        public int[] Price { get; set; }
        public int SubCode { get { return MessageTag.U_PROFILE; } }

        public ModelPlayList[] PlayLists { get; set; }

        public PlayListResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("PlayListResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new System.Exception("PlayListResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new System.Exception("PlayListResponse: Invalid Data 2 - Name");
            if (!operationResponse.Parameters.ContainsKey(Const.Data3)) throw new System.Exception("PlayListResponse: Invalid Data 3 - Slug");
            if (!operationResponse.Parameters.ContainsKey(Const.Data4)) throw new System.Exception("PlayListResponse: Invalid Data 4 - Enable");
            if (!operationResponse.Parameters.ContainsKey(Const.Data5)) throw new System.Exception("PlayListResponse: Invalid Data 5 - Start");
            if (!operationResponse.Parameters.ContainsKey(Const.Data6)) throw new System.Exception("PlayListResponse: Invalid Data 6 - End");
            if (!operationResponse.Parameters.ContainsKey(Const.Data7)) throw new System.Exception("PlayListResponse: Invalid Data 7 - Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data8)) throw new System.Exception("PlayListResponse: Invalid Data 8 - Price");

            try
            {

                this.Id = operationResponse[Const.Data1] as int[];
                this.Name = operationResponse[Const.Data2] as string[];
                this.Slug = operationResponse[Const.Data3] as string[];
                this.Enable = operationResponse[Const.Data4] as bool[];
                this.Start = operationResponse[Const.Data5] as int[];
                this.End = operationResponse[Const.Data6] as int[];
                this.Type = operationResponse[Const.Data7] as int[];
                this.Price = operationResponse[Const.Data8] as int[];

                this.PlayLists = new ModelPlayList[this.Name.Length];
                for (int i = 0; i < this.Name.Length; i++)
                {
                    ModelPlayList info = new ModelPlayList();
                    info.id = this.Id[i];
                    info.name = this.Name[i];
                    info.slug = this.Slug[i];
                    info.start = this.Start[i];
                    info.end = this.End[i];
                    info.type = (ModelPlayList.PlaylistType)this.Type[i];
                    info.price = this.Price[i];

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