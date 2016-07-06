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
                        ProfileResponse profile = new ProfileResponse(operationResponse);

                        // bind data to Me
                        if (profile.Id == int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId)) Me.Data = new Custom.Models.ModelUser() { id = profile.Id, username = profile.Username, nickname = profile.Nickname };

                        // call another event handler
                        if (onProfileResponse != null) onProfileResponse(profile);
                        break;

                    case MessageTag.G_PLAYLIST:
                        if (onPlaylistResponse != null) onPlaylistResponse(new PlayListResponse(operationResponse));
                        break;

                    case MessageTag.G_QUESTIONLIST:
                        if (onSongListResponse != null) onSongListResponse(new QuestionListResponse(operationResponse));
                        break;

                    case MessageTag.G_STATE_CHANGE:
                        if (onGameStateChangeResponse != null) onGameStateChangeResponse(new GameStateResponse(operationResponse));
                        break;

                    case MessageTag.G_READY_LIST:
                        if (onReadyListResponse != null) onReadyListResponse(new ReadyPlayersResponse(operationResponse));
                        break;

                    default:
                        Debug.LogFormat("<color=orange>Unknow Response: {0}</color>", operationResponse);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogFormat("<color=red>Error in {0}: {1} - [2]</color>", opCode, ex.Message, operationResponse);
            }

            return null;
        }

    }
}
#endif