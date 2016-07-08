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

        #region Properties

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
        [SerializeField]
        private UIButton[] btnOptions;

        [Header("Game End Panel")]
        [SerializeField]
        private GameObject panelGameEnd;

        [Header("Game Data")]
        // [SerializeField]
        private QuestionList[] QuestionLists;
        [SerializeField]
        private int downloadedAudio = 0;
        private int DownloadedAudio
        {
            get { return this.downloadedAudio; }
            set { this.downloadedAudio = value; if (value != 0 && value == this.QuestionLists.Length) { onDownloadCompleted(); } }
        }
        // [SerializeField]
        private SongGameState state = SongGameState.NONE;

        private int PlayedRound;
        private int TotalRound;
        private int PlayedQuestion;
        private int TotalQuestion;

        private float StartQuestionTime = 0f;
        private float BuzzTime = 0f;

        private int timePlayCountdown = 0;
        private Coroutine _playCountDown;

        #endregion

        void Awake()
        {
            EventDelegate.Add(btnLeaveRoom.onClick, onLeaveRoomClick);
            EventDelegate.Add(btnBuzz.onClick, onAnwserBuzzClick);
            EventDelegate.Add(txtAnwser.onSubmit, onAnwserTextSubmit);

            for (int i = 0; i < btnOptions.Length; i++)
            {
                int btnIndex = i; // fuck mono
                EventDelegate.Add(btnOptions[i].onClick, () => { onAnwserOptionClick(btnIndex); });
            }
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
            ResponseHandler.onAnwserOptionResponse += onAnwserOptionResponse;

            this.panelGameLobby.SetActive(true);
            this.textBuzzVisible();
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
            ResponseHandler.onAnwserOptionResponse -= onAnwserOptionResponse;
        }

        public void ChangeState(SongGameState state)
        {
            if (this.state == state) return;

            // Debug.LogFormat("State change to: {0}", state);

            if (state <= SongGameState.WAIT)
            {
                if (panelGameLobby.activeSelf == false) panelGameLobby.SetActive(true);
                if (panelGamePlay.activeSelf == true) panelGamePlay.SetActive(false);
                if (panelGameEnd.activeSelf == true) panelGameEnd.SetActive(false);
            }
            else if (state >= SongGameState.READY && state < SongGameState.END)
            {
                if (panelGameLobby.activeSelf == true) panelGameLobby.SetActive(false);
                if (panelGamePlay.activeSelf == false) panelGamePlay.SetActive(true);
                if (panelGameEnd.activeSelf == true) panelGameEnd.SetActive(false);
            }
            else if (state >= SongGameState.END)
            {
                if (panelGameLobby.activeSelf == true) panelGameLobby.SetActive(false);
                if (panelGamePlay.activeSelf == true) panelGamePlay.SetActive(false);
                if (panelGameEnd.activeSelf == false) panelGameEnd.SetActive(true);
            }

            this.state = state;
            if (this._playCountDown != null)
            {
                StopCoroutine(this._playCountDown);
                this.lblTimeCoundown.text = "0";
            }

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

        private void OnGameReady()
        {
            this.PlayedQuestion = 0;
            this.TotalQuestion = QuestionLists.Length;
            this.lblQuestion.text = "Ready to play";
        }

        private void OnGamePlaying()
        {
            for (int i = 0; i < btnOptions.Length; i++)
            {
                if (i == 0) btnOptions[i].GetComponentInChildren<UILabel>().text = QuestionLists[PlayedQuestion].Option1;
                if (i == 1) btnOptions[i].GetComponentInChildren<UILabel>().text = QuestionLists[PlayedQuestion].Option2;
                if (i == 2) btnOptions[i].GetComponentInChildren<UILabel>().text = QuestionLists[PlayedQuestion].Option3;
                if (i == 3) btnOptions[i].GetComponentInChildren<UILabel>().text = QuestionLists[PlayedQuestion].Option4;
            }

            timePlayCountdown = Rules.TIME_WAIT_PER_QUESTION / 1000;
            lblTimeCoundown.text = timePlayCountdown.ToString();
            _playCountDown = StartCoroutine(PlayCountDown());

            lblQuestion.text = QuestionLists[PlayedQuestion].Question;
            audioSource.clip = QuestionLists[PlayedQuestion].AudioClip;
            audioSource.Play();
            PlayedQuestion++;

            textBuzzVisible(btnBuzz.gameObject);
            StartCoroutine(WaitForSeconds(Rules.TIME_WAIT_PART_1, () =>
            {
                if (txtAnwser.gameObject.activeSelf == false)
                {
                    textBuzzVisible(); // disable buzz button and awnser input text
                }
            }));

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

        private IEnumerator WaitForSeconds(float time, Action callback) { yield return new WaitForSeconds(time); if (callback != null) callback(); }
        
        private IEnumerator PlayCountDown()
        {
            while (this.timePlayCountdown > 0)
            {
                yield return new WaitForSeconds(1);
                this.timePlayCountdown--;
                this.lblTimeCoundown.text = this.timePlayCountdown.ToString();
            }
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

            AudioClip clip = www.GetAudioClip(true, true);
            finishCallback(clip);
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

        private void textBuzzVisible(GameObject activeObject = null)
        {
            this.btnBuzz.gameObject.SetActive(false);
            this.txtAnwser.gameObject.SetActive(false);
            if (activeObject != null) activeObject.SetActive(true);
        }

        private void onAnwserBuzzClick()
        {
            if (this.state != SongGameState.PLAYING) return;
            RequestHandler.RequestAnwserBuzz();
        }

        private void onAnwserBuzzResponse(AnwserBuzzResponse response)
        {
            if (this.state != SongGameState.PLAYING) return;
            if (response.Id == Me.Data.id) { this.textBuzzVisible(this.txtAnwser.gameObject); }
        }

        private void onAnwserTextSubmit()
        {
            if (this.state != SongGameState.PLAYING) return;
            RequestHandler.RequestAnwserText(txtAnwser.value);
        }

        private void onAnwserTextResponse(AnwserTextResponse response)
        {
            if (this.state != SongGameState.PLAYING) return;
        }

        private void optionsButtonVisible(bool flag = true)
        {
            foreach (var btn in this.btnOptions)
            {
                if (flag == true) btn.gameObject.SetActive(false);
                else btn.gameObject.SetActive(true);
            }
        }

        private void onAnwserOptionClick(int btnIndex)
        {
            if (this.state != SongGameState.PLAYING) return;
            RequestHandler.RequestAnwserOption(btnIndex);
        }

        private void onAnwserOptionResponse(AnwserOptionResponse response)
        {
            if (this.state != SongGameState.PLAYING) return;
        }

    }
}