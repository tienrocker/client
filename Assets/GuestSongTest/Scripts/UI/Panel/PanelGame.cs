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
    using System.Linq;
    using Photon.LoadBalancing.Client.Data;
    using Photon.LoadBalancing.Custom.Common;
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
        private UIButton btnLeaveRoom;

        [Header("Game Play Panel")]
        [SerializeField]
        private GameObject panelGamePlay;
        [SerializeField]
        private UILabel lblTimeCoundown;
        [SerializeField]
        private UILabel lblQuestion;
        [SerializeField]
        private UIButton btnBuzz;
        [SerializeField]
        private UIInput txtAnwser;

        [Header("Game Data")]
        // [SerializeField]
        private QuestionList[] QuestionLists;
        // [SerializeField]
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
        // [SerializeField]
        private SongGameState state = SongGameState.NONE;

        // for visual
        // [SerializeField]
        private int PlayedRound;
        // [SerializeField]
        private int TotalRound;
        // [SerializeField]
        private int PlayedQuestion;
        // [SerializeField]
        private int TotalQuestion;

        private float StartQuestionTime = 0f;
        private float BuzzTime = 0f;

        #endregion

        void Awake()
        {
            EventDelegate.Add(btnLeaveRoom.onClick, onLeaveRoomClick);
            EventDelegate.Add(btnBuzz.onClick, onBuzzClick);
            EventDelegate.Add(txtAnwser.onSubmit, onTextAnwser);
        }

        public override void OnShow()
        {
            base.OnShow();
            if (PhotonNetwork.connected) txtDebug.text = "[FF0000]OnJoinedRoom with " + PhotonNetwork.room.playerCount + " Player(s) / [FFFFFF]" + LitJson.JsonMapper.ToJson(PhotonNetwork.room);

            NetworkManager.Instance.onPhotonPlayerConnected += onPhotonPlayerConnected;
            NetworkManager.Instance.onPhotonPlayerDisconnected += onPhotonPlayerDisconnected;
            ResponseHandler.onSongListResponse += onSongListResponse;
            ResponseHandler.onGameStateChangeResponse += onGameStateChangeResponse;
            ResponseHandler.onReadyListResponse += onReadyListResponse;
            ResponseHandler.onAnwserBuzzResponse += onAnwserBuzzResponse;
            ResponseHandler.onAnwserTextResponse += onAnwserTextResponse;

            panelGameLobby.SetActive(true);
        }

        public override void OnHide()
        {
            base.OnHide();

            NetworkManager.Instance.onPhotonPlayerConnected -= onPhotonPlayerConnected;
            NetworkManager.Instance.onPhotonPlayerDisconnected -= onPhotonPlayerDisconnected;
            ResponseHandler.onSongListResponse -= onSongListResponse;
            ResponseHandler.onGameStateChangeResponse -= onGameStateChangeResponse;
            ResponseHandler.onReadyListResponse -= onReadyListResponse;
            ResponseHandler.onAnwserBuzzResponse -= onAnwserBuzzResponse;
            ResponseHandler.onAnwserTextResponse -= onAnwserTextResponse;
        }

        public void ChangeState(SongGameState state)
        {
            if (this.state == state) return;

            // Debug.LogFormat("State change to: {0}", state);

            if (state <= SongGameState.WAIT)
            {
                if (panelGameLobby.activeSelf == true) panelGameLobby.SetActive(false);
                if (panelGamePlay.activeSelf == false) panelGamePlay.SetActive(true);
            }
            else if (state >= SongGameState.READY)
            {
                if (panelGameLobby.activeSelf == true) panelGameLobby.SetActive(false);
                if (panelGamePlay.activeSelf == false) panelGamePlay.SetActive(true);
            }

            this.state = state;
            if (this._playCountDown != null) StopCoroutine(this._playCountDown);

            switch (this.state)
            {
                case SongGameState.READY:

                    this.OnGameReady(); // just reset few things
                    break;

                case SongGameState.PLAYING:

                    this.OnGamePlaying(); // wait to player chose or type awnser
                    break;

                case SongGameState.RESULT:

                    this.OnGameCalculateResult();
                    break;

                case SongGameState.WAIT_NEXT_QUESTION:

                    this.OnWaitNextPlay();
                    break;

                case SongGameState.WAIT_NEXT_ROUND:

                    this.OnWaitNextRound();
                    break;

                case SongGameState.END:

                    this.OnGameEnd();
                    break;
            }
        }

        public int timePlayCountdown = 0;
        private void OnGameReady()
        {
            this.PlayedQuestion = 0;
            this.TotalQuestion = QuestionLists.Length;
            this.lblQuestion.text = "Ready to play";
        }

        private void OnGamePlaying()
        {
            this.timePlayCountdown = Rules.TIME_WAIT_PER_QUESTION / 1000;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            this._playCountDown = StartCoroutine(PlayCountDown());

            this.lblQuestion.text = QuestionLists[this.PlayedQuestion].Question;
            this.audioSource.clip = QuestionLists[this.PlayedQuestion].AudioClip;
            this.audioSource.Play();
            this.PlayedQuestion++;
        }

        private void OnGameCalculateResult()
        {
            this.timePlayCountdown = Rules.TIME_WAIT_TO_SHOW_RESULT / 1000;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            this._playCountDown = StartCoroutine(PlayCountDown());

            this.audioSource.Stop();
            this.lblQuestion.text = "Show result";
        }

        private void OnWaitNextPlay()
        {
            this.timePlayCountdown = Rules.TIME_WAIT_TO_NEXT_QUESTION / 1000;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            this._playCountDown = StartCoroutine(PlayCountDown());

            this.lblQuestion.text = "Wait next play";
        }

        private void OnWaitNextRound()
        {
            this.timePlayCountdown = Rules.TIME_WAIT_TO_NEXT_ROUND / 1000;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            this._playCountDown = StartCoroutine(PlayCountDown());

            this.PlayedRound++;
            this.lblQuestion.text = "Wait next round";
        }

        private void OnGameEnd()
        {
            this.timePlayCountdown = Rules.TIME_WAIT_TO_QUIT / 1000;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            this._playCountDown = StartCoroutine(PlayCountDown());

            this.lblQuestion.text = "Wait quit room";
        }

        private void onSongListResponse(QuestionListResponse response)
        {
            string _debug = LitJson.JsonMapper.ToJson(response.QuestionLists);
            if (txtDebug != null) txtDebug.text = _debug;
            else Debug.Log(_debug);

            // download and send message to server when downloaded
            this.QuestionLists = response.QuestionLists;
            this.PlayedRound = response.PlayedRound;
            this.TotalRound = response.TotalRound;
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
            RequestHandler.RequestReadyPlay(); // send message server, user ready to play
        }

        private Coroutine _playCountDown;
        private IEnumerator PlayCountDown()
        {
            yield return new WaitForSeconds(1);
            this.timePlayCountdown--;
            this.lblTimeCoundown.text = this.timePlayCountdown.ToString();

            // if (this.timePlayCountdown == 8) { }
            if (this.timePlayCountdown > 0) this._playCountDown = StartCoroutine(PlayCountDown());
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
            Debug.LogFormat("Player {0} ready", String.Join(", ", response.Id.Select(x => x.ToString()).ToArray()));
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

        private void onBuzzClick() { if (this.state == SongGameState.PLAYING) { RequestHandler.RequestAnwserBuzz(); } }

        private void onAnwserBuzzResponse(AnwserBuzzResponse response)
        {
            if (response.Id == Me.Data.id) { this.txtAnwser.gameObject.SetActive(true); }
        }

        private void onTextAnwser() { if (this.state == SongGameState.PLAYING) { RequestHandler.RequestAnwserText(txtAnwser.value); } }

        private void onAnwserTextResponse(AnwserTextResponse response)
        {
        }


    }
}