#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Logic
{
    using System.Collections.Generic;
    using Photon.LoadBalancing.Custom.Server.Operations.Responses;
    using Photon.LoadBalancing.Custom.Common.Quiz;
    using Hive;
    using Common;
    using GameServer;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using SocketServer;

    public class SongGame : BaseGame
    {

        public SongGame(HivePeer peer, Game room) : base(peer, room)
        {
            this.PlayListId = (int)room.Properties.GetProperty(Const.GameCustomProperty).Value;
        }

        private SongGameState _state { get; set; }
        public SongGameState state { get { return _state; } set { if (_state != value) { _state = value; if (this.peers.Count > 0) ResponseHandler.SetState(this.peers, _state); } } }

        public int PlayListId { get; set; }
        public int PlayedRound { get; set; }
        public List<int> ReadyPlayerIds = new List<int>();
        public List<QuestionListResponse> QuestionList = new List<QuestionListResponse>();

        public override void OnPlayerJoinGame(HivePeer peer)
        {
            base.OnPlayerJoinGame(peer);

            if (this.peers.Count >= Rules.MIN_USER_NUMBER)
            {
                game.IsVisible = false; // lock game and another can't join game
                game.ExecutionFiber.Enqueue(() => OnAllPlayerJoined()); // change state
            }
        }

        public override void OnPlayerLeaveGame(HivePeer peer)
        {
            base.OnPlayerLeaveGame(peer);

            if (this.state >= SongGameState.WAIT)
            {
                int UserId = int.Parse(peer.UserId);
                if (this.ReadyPlayerIds.Contains(UserId)) { this.ReadyPlayerIds.Remove(UserId); }
            }
        }

        public virtual void OnAllPlayerJoined()
        {
            if (state != SongGameState.NONE || state != SongGameState.END) { log.ErrorFormat("Game OnAllPlayerJoined invalid state: {0}", state); }

            state = SongGameState.WAIT; // reset
            PlayedRound = 0; // reset

            ResponseHandler.SetState(this.peers, this.state); // send game state
            ResponseHandler.QuestionList(this.peers, this); // send song list

            game.ExecutionFiber.Enqueue(() => OnGameWaitToDownload()); // change state to wait
        }

        public virtual void OnGameWaitToDownload()
        {
            var data = this;
            Task.Factory.StartNew(() =>
            {
                int waittime = 0;
                while (true)
                {

                    if (data == null || data.peers.Count == 0)
                    {
                        break;
                    }

                    if (data.state == SongGameState.READY)
                    {
                        break;
                    }

                    // check user slow download and kick out of room
                    if (waittime >= Rules.TIME_WAIT_TO_DOWNLOAD)
                    {
                        // check and kick here
                        // data.state = SongGameState.NONE;
                    }

                    waittime += Rules.TIME_WAIT_TO_DOWNLOAD_STEP;
                    Thread.Sleep(Rules.TIME_WAIT_TO_DOWNLOAD_STEP); // wait all player ready
                }
            });
        }

        public override void OnPlayerReady(HivePeer peer)
        {
            if (this.state != SongGameState.WAIT) return; // wrong request

            // send message to other player
            this.ReadyPlayerIds.Add(int.Parse(peer.UserId));
            ResponseHandler.ReadyList(this.peers, ReadyPlayerIds);

            if (this.ReadyPlayerIds.Count == this.peers.Count)
            {
                this.state = SongGameState.READY;
                this.game.ExecutionFiber.Enqueue(() => { this.OnGameReady(); }); // change state to ready
            }
        }

        public virtual void OnGameReady()
        {

        }

        public override void ExecuteOperation(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            base.ExecuteOperation(peer, operationRequest, sendParameters);


            int opCode = 0;
            object tagCode;
            if (operationRequest.Parameters.TryGetValue(MessageTag.KINGPLAY_OPERATION_TAG, out tagCode)) { opCode = int.Parse(tagCode.ToString()); }

            if (log.IsDebugEnabled)
            {
                //log.DebugFormat("Received game operation on peer without a game: peerId={0}", this.ConnectionId);
            }

            switch (opCode)
            {
                case MessageTag.G_READY:
                    OnPlayerReady(peer);
                    break;

                default:
                    break;
            }

        }
    }
}
#endif