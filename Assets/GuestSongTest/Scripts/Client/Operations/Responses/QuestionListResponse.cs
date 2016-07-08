#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using UnityEngine;
    using GuestSong;

    [Serializable]
    public class QuestionList
    {
        public int Id;
        public string Question;
        public string Option1;
        public string Option2;
        public string Option3;
        public string Option4;
        public string Url;
        public string BundleName;
        public string AssetName;
        public AudioClip AudioClip;
    }

    public class QuestionListResponse
    {
        public int PlayedRound;
        public int TotalRound;
        public QuestionList[] QuestionLists;
        public int SubCode { get { return MessageTag.G_QUESTIONLIST; } }

        public QuestionListResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("QuestionListResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new System.Exception("QuestionListResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new System.Exception("QuestionListResponse: Invalid Data 2 - Question");
            if (!operationResponse.Parameters.ContainsKey(Const.Data3)) throw new System.Exception("QuestionListResponse: Invalid Data 3 - Option1");
            if (!operationResponse.Parameters.ContainsKey(Const.Data4)) throw new System.Exception("QuestionListResponse: Invalid Data 4 - Option2");
            if (!operationResponse.Parameters.ContainsKey(Const.Data5)) throw new System.Exception("QuestionListResponse: Invalid Data 5 - Option3");
            if (!operationResponse.Parameters.ContainsKey(Const.Data6)) throw new System.Exception("QuestionListResponse: Invalid Data 6 - Option4");
            if (!operationResponse.Parameters.ContainsKey(Const.Data7)) throw new System.Exception("QuestionListResponse: Invalid Data 7 - Url");
            if (!operationResponse.Parameters.ContainsKey(Const.Data8)) throw new System.Exception("QuestionListResponse: Invalid Data 8 - BundleName");
            if (!operationResponse.Parameters.ContainsKey(Const.Data9)) throw new System.Exception("QuestionListResponse: Invalid Data 9 - AssetName");
            if (!operationResponse.Parameters.ContainsKey(Const.Data10)) throw new System.Exception("QuestionListResponse: Invalid Data 10 - PlayedRound");
            if (!operationResponse.Parameters.ContainsKey(Const.Data11)) throw new System.Exception("QuestionListResponse: Invalid Data 11 - TotalRound");

            try
            {

                int[] Id = operationResponse[Const.Data1] as int[];
                string[] Question = operationResponse[Const.Data2] as string[];
                string[] Option1 = operationResponse[Const.Data3] as string[];
                string[] Option2 = operationResponse[Const.Data4] as string[];
                string[] Option3 = operationResponse[Const.Data5] as string[];
                string[] Option4 = operationResponse[Const.Data6] as string[];
                string[] Url = operationResponse[Const.Data7] as string[];
                string[] BundleName = operationResponse[Const.Data8] as string[];
                string[] AssetName = operationResponse[Const.Data9] as string[];
                this.PlayedRound = (int)operationResponse[Const.Data10];
                this.TotalRound = (int)operationResponse[Const.Data11];

                this.QuestionLists = new QuestionList[Id.Length];
                for (int i = 0; i < Id.Length; i++)
                {
                    QuestionList info = new QuestionList();
                    info.Id = Id[i];
                    info.Question = Question[i];
                    info.Option1 = Option1[i];
                    info.Option2 = Option2[i];
                    info.Option3 = Option3[i];
                    info.Option4 = Option4[i];
                    info.Url = Url[i];
                    info.BundleName = BundleName[i];
                    info.AssetName = AssetName[i];

                    this.QuestionLists[i] = info;
                }

                if (NetworkManager.responseDebug) Debug.LogFormat("List question: {0} ", LitJson.JsonMapper.ToJson(this.QuestionLists));
            }
            catch
            {
                throw new Exception("QuestionListResponse: Invalid Data Type");
            }
        }
    }
}
#endif