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
        [SerializeField]
        private bool autoJoinGame = false;

        void Awake()
        {
            EventDelegate.Add(btnProfile.onClick, onProfile);
            EventDelegate.Add(btnPlayList.onClick, onPlayList);
        }

        public override void OnShow()
        {
            base.OnShow();
            ResponseHandler.onProfileResponse += OnProfileResponse;
            ResponseHandler.onPlaylistResponse += OnPlaylistResponse;
            this.onPlayList();
        }

        public override void OnHide()
        {
            base.OnHide();
            ResponseHandler.onProfileResponse -= OnProfileResponse;
            ResponseHandler.onPlaylistResponse -= OnPlaylistResponse;
        }

        void onProfile()
        {
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
            RequestHandler.RequestPlayList();
        }

        private void OnPlaylistResponse(PlayListResponse response)
        {
            string _debug = LitJson.JsonMapper.ToJson(response.PlayLists);
            if (txtDebug != null) txtDebug.text = _debug;
            else Debug.Log(_debug);

            playListView.AddItem(response.PlayLists);
            if (this.autoJoinGame) { playListView.firstItem.SendMessage("OnClick"); }
        }
    }
}