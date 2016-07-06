using UnityEngine;
using System.Collections;
using Photon.LoadBalancing.Client;
using Photon.LoadBalancing.Client.Operations.Responses;
using System;

namespace GuestSong
{
    public class PanelLobby : PanelBase
    {
        [SerializeField]
        private UILabel txtDebug;

        [SerializeField]
        private UIButton btnProfile;

        [SerializeField]
        private UIButton btnPlayList;

        [SerializeField]
        private PlayListView playListView;

        void Awake()
        {
            EventDelegate.Add(btnProfile.onClick, onProfile);
            EventDelegate.Add(btnPlayList.onClick, onPlayList);
        }

        void onProfile()
        {
            ResponseHandler.onProfileResponse -= OnProfileResponse;
            ResponseHandler.onProfileResponse += OnProfileResponse;
            RequestHandler.RequestProfile(int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId));
        }

        private void OnProfileResponse(ProfileResponse response)
        {
            string _debug = LitJson.JsonMapper.ToJson(response);
            if (txtDebug != null) txtDebug.text = _debug;
            else Debug.Log(_debug);
        }

        void onPlayList()
        {
            ResponseHandler.onPlaylistResponse -= OnPlaylistResponse;
            ResponseHandler.onPlaylistResponse += OnPlaylistResponse;
            RequestHandler.RequestPlayList(int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId));
        }

        private void OnPlaylistResponse(PlayListResponse response)
        {
            string _debug = LitJson.JsonMapper.ToJson(response.PlayLists);
            if (txtDebug != null) txtDebug.text = _debug;
            else Debug.Log(_debug);

            playListView.AddItem(response.PlayLists);
        }
    }
}