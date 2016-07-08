#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client
{
    using ExitGames.Client.Photon;
    using Operations.Responses;
    using System;
    using Custom.Common;
    using UnityEngine;
    using Data;

    public class ResponseHandler
    {
        public static Action<ProfileResponse> onProfileResponse;
        public static Action<PlayListResponse> onPlaylistResponse;
        public static Action<QuestionListResponse> onSongListResponse;
        public static Action<GameStateResponse> onGameStateChangeResponse;
        public static Action<ReadyPlayersResponse> onReadyListResponse;
        public static Action<AnwserBuzzResponse> onAnwserBuzzResponse;
        public static Action<AnwserTextResponse> onAnwserTextResponse;
        public static Action<AnwserOptionResponse> onAnwserOptionResponse;

        public static OperationResponse TryPaser(OperationResponse operationResponse)
        {
            int opCode = 0;
            object tagCode;
            if (operationResponse.Parameters.TryGetValue(MessageTag.KINGPLAY_OPERATION_TAG, out tagCode)) { opCode = (int)tagCode; }

            try
            {
                switch (opCode)
                {
                    case MessageTag.U_PROFILE:
                        {
                            ProfileResponse response = new ProfileResponse(operationResponse);
                            if (response.Id == int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId)) Me.Data = new Custom.Models.ModelUser() { id = response.Id, username = response.Username, nickname = response.Nickname }; // bind data to Me
                            if (onProfileResponse != null) onProfileResponse(response); // call another event handler
                        }
                        break;

                    case MessageTag.G_PLAYLIST:
                        {
                            var response = new PlayListResponse(operationResponse);
                            if (onPlaylistResponse != null) onPlaylistResponse(response);
                        }
                        break;

                    case MessageTag.G_QUESTIONLIST:
                        {
                            var response = new QuestionListResponse(operationResponse);
                            if (onSongListResponse != null) onSongListResponse(response);
                        }
                        break;

                    case MessageTag.G_STATE_CHANGE:
                        {
                            var response = new GameStateResponse(operationResponse);
                            if (onGameStateChangeResponse != null) onGameStateChangeResponse(response);
                        }
                        break;

                    case MessageTag.G_READY_LIST:
                        {
                            var response = new ReadyPlayersResponse(operationResponse);
                            if (onReadyListResponse != null) onReadyListResponse(response);
                        }
                        break;

                    case MessageTag.G_ANWSER_BUZZ:
                        {
                            var response = new AnwserBuzzResponse(operationResponse);
                            if (onAnwserBuzzResponse != null) onAnwserBuzzResponse(response);
                        }
                        break;

                    case MessageTag.G_ANWSER_TEXT:
                        {
                            var response = new AnwserTextResponse(operationResponse);
                            if (onAnwserTextResponse != null) onAnwserTextResponse(response);
                        }
                        break;

                    case MessageTag.G_ANWSER_OPTION:
                        {
                            var response = new AnwserOptionResponse(operationResponse);
                            if (onAnwserOptionResponse != null) onAnwserOptionResponse(response);
                        }
                        break;

                    default:
                        Debug.LogFormat("<color=orange>Unknow Response: {0}</color>", operationResponse);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogFormat("<color=red>Error in {0}: {1} - {2}</color>", opCode, ex.Message, operationResponse);
            }

            return null;
        }

    }
}
#endif