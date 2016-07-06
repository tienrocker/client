namespace GuestSong
{
    using System;
    using Photon.LoadBalancing.Client;
    using Photon.LoadBalancing.Client.Operations.Responses;
    using Photon.LoadBalancing.Custom.Common.Quiz;
    using UnityEngine;
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using System.Collections;
    using System.Threading;
    using System.Collections.Generic;
    public class PanelGame : PanelBase
    {

        #region properties
        [Header("Debug")]
        [SerializeField]
        private UILabel txtDebug;
        [SerializeField]
        private AudioSource audioSource;

        [Header("Game Lobby Panel")]
        [SerializeField]
        private GameObject panelGameLobby;
        [SerializeField]
        private GameObject panelGameScene;
        [SerializeField]
        private UIButton btnLeaveRoom;

        [Header("Game Play Panel")]
        [SerializeField]
        private GameObject panelGamePlay;

        [Header("Game Data")]
        [SerializeField]
        private QuestionList[] QuestionLists;
        [SerializeField]
        private int _downloadedAudio = 0;
        private int DownloadedAudio
        {
            get { return this._downloadedAudio; }
            set
            {
                this._downloadedAudio = value;
                if (value != 0 && value == this.QuestionLists.Length) { onDownloadCompleted(); }
            }
        }
        [SerializeField]
        private SongGameState state = SongGameState.NONE;
        #endregion

        public override void OnShow()
        {
            base.OnShow();
            if (PhotonNetwork.connected) txtDebug.text = "[FF0000]OnJoinedRoom with " + PhotonNetwork.room.playerCount + " Player(s) / [FFFFFF]" + LitJson.JsonMapper.ToJson(PhotonNetwork.room);

            EventDelegate.Add(btnLeaveRoom.onClick, onLeaveRoomClick);

            NetworkManager.Instance.onPhotonPlayerConnected += onPhotonPlayerConnected;
            NetworkManager.Instance.onPhotonPlayerDisconnected += onPhotonPlayerDisconnected;
            ResponseHandler.onSongListResponse += onSongListResponse;
            ResponseHandler.onGameStateChangeResponse += onGameStateChangeResponse;
            ResponseHandler.onReadyListResponse += onReadyListResponse;

            panelGameLobby.SetActive(true);
        }

        public override void OnHide()
        {
            base.OnHide();

            EventDelegate.Remove(btnLeaveRoom.onClick, onLeaveRoomClick);

            NetworkManager.Instance.onPhotonPlayerConnected -= onPhotonPlayerConnected;
            NetworkManager.Instance.onPhotonPlayerDisconnected -= onPhotonPlayerDisconnected;
            ResponseHandler.onSongListResponse -= onSongListResponse;
            ResponseHandler.onGameStateChangeResponse -= onGameStateChangeResponse;
            ResponseHandler.onReadyListResponse -= onReadyListResponse;
        }

        public void ChangeState(SongGameState state)
        {
            if (this.state == state) return;

            Debug.LogFormat("State change to: {0}", state);

            if (state <= SongGameState.WAIT) panelGameLobby.SetActive(false);
            if (state >= SongGameState.READY) panelGameScene.SetActive(false);

            if (state == SongGameState.NONE || state == SongGameState.WAIT) panelGameLobby.SetActive(true);
            if (state == SongGameState.READY) panelGameScene.SetActive(true);

            this.state = state;

            switch (this.state)
            {
                case SongGameState.NONE:
                    break;
                case SongGameState.WAIT:
                    break;
                case SongGameState.READY:

                    break;
            }
        }

        private void onSongListResponse(QuestionListResponse response)
        {
            string _debug = LitJson.JsonMapper.ToJson(response.QuestionLists);
            if (txtDebug != null) txtDebug.text = _debug;
            else Debug.Log(_debug);

            // download and send message to server when downloaded
            this.QuestionLists = response.QuestionLists;
            this.DownloadedAudio = 0;

            for (int i = 0; i < this.QuestionLists.Length; i++)
            {
                var question = this.QuestionLists[i];
                StartCoroutine(DownloadAudioClip(question.Url, (audioClip) =>
                {
                    question.AudioClip = audioClip;
                    this.DownloadedAudio++;
                }));
            }
        }

        private void onDownloadCompleted()
        {
            RequestHandler.RequestReadyPlay(int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId)); // send message server, user ready to play
        }

        private IEnumerator DownloadAudioClip(string clipUrl, Action<AudioClip> finishCallback, Action<float> processCallback = null)
        {
            while (!Caching.ready) yield return null;

            var www = new WWW(clipUrl);

            while (!www.isDone)
            {
                if (processCallback != null) processCallback(www.progress);
                yield return null;
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                yield return null;
            }

            finishCallback(www.audioClip);
        }

        private void onReadyListResponse(ReadyPlayersResponse response)
        {
            throw new NotImplementedException();
        }

        private void onGameStateChangeResponse(GameStateResponse gamestate)
        {
            ChangeState(gamestate.State);
        }

        private void onPhotonPlayerDisconnected(PhotonPlayer response)
        {
            txtDebug.text = "[FF0000]OnJoinedRoom with " + PhotonNetwork.room.playerCount + " Player(s) / [FFFFFF]" + LitJson.JsonMapper.ToJson(PhotonNetwork.room);
        }

        private void onPhotonPlayerConnected(PhotonPlayer response)
        {
            txtDebug.text = "[FF0000]OnJoinedRoom with " + PhotonNetwork.room.playerCount + " Player(s) / [FFFFFF]" + LitJson.JsonMapper.ToJson(PhotonNetwork.room);
        }

        private void onLeaveRoomClick()
        {
            PhotonNetwork.LeaveRoom();
        }

    }
}