namespace GuestSong
{

    using System;
    using UnityEngine;
    using Photon.LoadBalancing.Custom;
    using ExitGames.Client.Photon;
    using Photon.LoadBalancing.Client;
    /// <summary>
    /// Network manager. Connect, join a random room or create one if none or all full.
    /// </summary>
    public class NetworkManager : Photon.PunBehaviour
    {

        #region Public Variables

        public static NetworkManager Instance;

        #endregion

        #region Private Variables

        [SerializeField]
        private string _masterIP = "";

#if !UNITY_WEBGL
        [SerializeField]
        private int _masterPort = 5055;
#endif

#if UNITY_WEBGL
        [SerializeField]
        private int _masterWebsocketPort = 9090;
#endif

#if !UNITY_WEBGL
        [SerializeField]
        private ConnectionProtocol Protocol = ConnectionProtocol.Udp;
#else
        [SerializeField]
        private ConnectionProtocol Protocol = ConnectionProtocol.WebSocket;
#endif

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
        /// </summary>
        [SerializeField]
        private string _gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        public Action<string> onCustomAuthenticationFailed;
        public Action<PhotonPlayer> onPhotonPlayerConnected;
        public Action<PhotonPlayer> onPhotonPlayerDisconnected;
        public Action<object[]> onPhotonRandomJoinFailed;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            Instance = this;

            // #NotImportant
            // Force LogLevel
            PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;

            // #Critical
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.networkingPeer.DebugOut = ExitGames.Client.Photon.DebugLevel.WARNING;

            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;
        }

        void Start()
        {
            // show login panel, call in start function to prevent singleton load index
            PanelManager.Instance.Show(PanelManager.PanelType.LOGIN);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process. 
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect(string username, string password, bool register = false, string nickname = "")
        {
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = true;

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                LogFeedback("Joining Room...");

                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                // PhotonNetwork.JoinRandomRoom();
                PhotonNetwork.JoinLobby();
            }
            else {

                LogFeedback("Connecting...");
                PanelManager.Instance.Show(PanelManager.PanelType.LOADING);

                // Add custom authentication
                PhotonNetwork.AuthValues = new AuthenticationValues() { AuthType = CustomAuthenticationType.Custom };
                PhotonNetwork.AuthValues.SetAuthPostData(new AuthenticationData() { username = username, password = password, nickname = nickname, register = register, type = AuthenticationData.Type.DIRECT }.Serialize());

                // Critical, we must first and foremost connect to Photon Online Server.
#if UNITY_WEBGL
                if (this.Protocol != ConnectionProtocol.WebSocket || this.Protocol != ConnectionProtocol.WebSocketSecure) this.Protocol = ConnectionProtocol.WebSocket;
                PhotonNetwork.SwitchToProtocol(this.Protocol);
                PhotonNetwork.ConnectToMaster(string.Format("ws://{0}", this._masterIP), this._masterWebsocketPort, "", this._gameVersion);
#else
                PhotonNetwork.SwitchToProtocol(this.Protocol);
                PhotonNetwork.ConnectToMaster(this._masterIP, this._masterPort, "", this._gameVersion);
#endif
            }
        }

        /// <summary>
        /// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void LogFeedback(string message)
        {
            // add new messages as a new line and at the bottom of the log.
            // var textlog = System.Environment.NewLine + message;
            Debug.Log(message);
        }

        #endregion

        #region Photon.PunBehaviour CallBacks

        // below, we implement some callbacks of PUN
        // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.photonengine.com/dashboard)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            LogFeedback("<Color=Red>OnCustomAuthenticationFailed</Color>: " + debugMessage);
            base.OnCustomAuthenticationFailed(debugMessage);

            if (onCustomAuthenticationFailed != null) onCustomAuthenticationFailed(debugMessage);
        }

        /// <summary>
        /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            LogFeedback("OnConnectedToMaster:" + PhotonNetwork.networkingPeer.MasterServerAddress);

            // we don't want to do anything if we are not attempting to join a room. 
            // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
            // we don't want to do anything.
            if (isConnecting)
            {
                LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
                LogFeedback("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
                // PhotonNetwork.JoinRandomRoom();
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();

            // get user profile
            RequestHandler.RequestProfile(int.Parse(PhotonNetwork.networkingPeer.mLocalActor.userId));

            PanelManager.Instance.Show(PanelManager.PanelType.LOBBY);
        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public override void OnJoinedRoom()
        {
            LogFeedback("<Color=Green>OnJoinedRoom</Color> with name " + PhotonNetwork.room.name + " and have " + PhotonNetwork.room.playerCount + " Player(s)");
            PanelManager.Instance.Show(PanelManager.PanelType.GAME);
            base.OnJoinedRoom();
        }

        /// <summary>
        /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            if (onPhotonPlayerConnected != null) onPhotonPlayerConnected(newPlayer);
            base.OnPhotonPlayerConnected(newPlayer);
        }

        /// <summary>
        /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
        /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
        /// of several seconds.
        /// </remarks>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            if (onPhotonPlayerDisconnected != null) onPhotonPlayerDisconnected(otherPlayer);
            base.OnPhotonPlayerDisconnected(otherPlayer);
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            LogFeedback("<Color=Red>OnPhotonRandomJoinFailed</Color>: Next -> Create a new Room");
            if (onPhotonRandomJoinFailed != null) onPhotonRandomJoinFailed(codeAndMsg);
            base.OnPhotonRandomJoinFailed(codeAndMsg);
        }

        /// <summary>
        /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
        /// The reason for the error is provided as DisconnectCause.
        /// </remarks>
        public override void OnConnectionFail(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnConnectionFail: " + cause + "</Color>");
            base.OnConnectionFail(cause);
        }

        /// <summary>
        /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// This is called when no connection could be established at all.
        /// It differs from OnConnectionFail, which is called when an existing connection fails.
        /// </remarks>
        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            LogFeedback("<Color=Red>OnFailedToConnectToPhoton: " + cause + "</Color>");
            base.OnFailedToConnectToPhoton(cause);
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        /// <remarks>
        /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
        /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
        /// </remarks>
        public override void OnDisconnectedFromPhoton()
        {
            LogFeedback("<Color=Red>OnDisconnectedFromPhoton</Color>");

            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            isConnecting = false;

            PanelManager.Instance.Show(PanelManager.PanelType.LOGIN);
        }

        /// <summary>
        /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
        /// </summary>
        /// <remarks>
        /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
        /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
        /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
        /// </remarks>
        public override void OnPhotonMaxCccuReached()
        {
            base.OnPhotonMaxCccuReached();
            Debug.LogError("OnPhotonMaxCccuReached");
        }
        #endregion

    }
}