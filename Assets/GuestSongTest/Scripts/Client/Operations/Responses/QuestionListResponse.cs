#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;
    using UnityEngine;

    [Serializable]
    public class QuestionList
    {
        public int Id;
        public string Question;
        public string Option1;
        public string Option2;
        public string Option3;
        public string Option4;
        public int Answer;
        public string Url;
        public string BundleName;
        public string AssetName;
        public AudioClip AudioClip;
    }

    public class QuestionListResponse
    {
        public int[] Id { get; set; }
        public string[] Question { get; set; }
        public string[] Option1 { get; set; }
        public string[] Option2 { get; set; }
        public string[] Option3 { get; set; }
        public string[] Option4 { get; set; }
        public int[] Answer { get; set; }
        public string[] Url { get; set; }
        public string[] BundleName { get; set; }
        public string[] AssetName { get; set; }
        public int SubCode { get { return MessageTag.G_QUESTIONLIST; } }

        public QuestionList[] QuestionLists;

        public QuestionListResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new System.Exception("SongListResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new System.Exception("SongListResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new System.Exception("SongListResponse: Invalid Data 2 - Question");
            if (!operationResponse.Parameters.ContainsKey(Const.Data3)) throw new System.Exception("SongListResponse: Invalid Data 3 - Option1");
            if (!operationResponse.Parameters.ContainsKey(Const.Data4)) throw new System.Exception("SongListResponse: Invalid Data 4 - Option2");
            if (!operationResponse.Parameters.ContainsKey(Const.Data5)) throw new System.Exception("SongListResponse: Invalid Data 5 - Option3");
            if (!operationResponse.Parameters.ContainsKey(Const.Data6)) throw new System.Exception("SongListResponse: Invalid Data 6 - Option4");
            if (!operationResponse.Parameters.ContainsKey(Const.Data7)) throw new System.Exception("SongListResponse: Invalid Data 7 - Answer");
            if (!operationResponse.Parameters.ContainsKey(Const.Data8)) throw new System.Exception("SongListResponse: Invalid Data 8 - Url");
            if (!operationResponse.Parameters.ContainsKey(Const.Data9)) throw new System.Exception("SongListResponse: Invalid Data 9 - BundleName");
            if (!operationResponse.Parameters.ContainsKey(Const.Data10)) throw new System.Exception("SongListResponse: Invalid Data 10 - AssetName");

            try
            {

                this.Id = operationResponse[Const.Data1] as int[];
                this.Question = operationResponse[Const.Data2] as string[];
                this.Option1 = operationResponse[Const.Data3] as string[];
                this.Option2 = operationResponse[Const.Data4] as string[];
                this.Option3 = operationResponse[Const.Data5] as string[];
                this.Option4 = operationResponse[Const.Data6] as string[];
                this.Answer = operationResponse[Const.Data7] as int[];
                this.Url = operationResponse[Const.Data8] as string[];
                this.BundleName = operationResponse[Const.Data9] as string[];
                this.AssetName = operationResponse[Const.Data10] as string[];

                this.QuestionLists = new QuestionList[this.Id.Length];
                for (int i = 0; i < this.Id.Length; i++)
                {
                    QuestionList info = new QuestionList();
                    info.Id = this.Id[i];
                    info.Question = this.Question[i];
                    info.Option1 = this.Option1[i];
                    info.Option2 = this.Option2[i];
                    info.Option3 = this.Option3[i];
                    info.Option4 = this.Option4[i];
                    info.Answer = this.Answer[i];
                    info.Url = this.Url[i];
                    info.BundleName = this.BundleName[i];
                    info.AssetName = this.AssetName[i];

                    this.QuestionLists[i] = info;
                }

            }
            catch
            {
                throw new Exception("SongListResponse: Invalid Data Type");
            }
        }
    }
}
#endif