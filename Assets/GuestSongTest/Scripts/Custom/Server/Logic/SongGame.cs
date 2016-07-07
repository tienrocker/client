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
    using MasterServer;
    using Operations.Requests;
    using Database;
    using Song;
    using Models;
    using System.Linq;

    public class SongGame : BaseGame
    {
        public SongGame(HivePeer peer, Game room) : base(peer, room)
        {
            this.PlayListId = (int)room.Properties.GetProperty(Const.GameCustomProperty).Value;
            this.total_round = Rules.ROUND_NUMBER;
            this.round_index = 0;
        }

        private SongGameState _state { get; set; }
        public SongGameState state { get { return _state; } set { if (_state != value) { _state = value; if (this.peers.Count > 0) ResponseHandler.GameStateResponse(this.peers, _state); } } }

        public int PlayListId { get; set; }
        public List<int> ReadyPlayerIds = new List<int>();

        public int round_index { get; set; }
        public int total_round { get; set; }

        public int question_index { get; set; }
        public int total_question { get; set; }

        public List<PlayerAnswer> players_answer = new List<PlayerAnswer>();

        public List<ModelQuestion> questions = null;
        public ModelQuestion current_question = null;

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
            if (state != SongGameState.NONE || state != SongGameState.WAIT_NEXT_ROUND) { log.ErrorFormat("Game OnAllPlayerJoined invalid state: {0}", state); }

            this.state = SongGameState.WAIT; // reset
            this.ReadyPlayerIds = new List<int>();

            ResponseHandler.GameStateResponse(this.peers, this.state); // send game state
            ResponseHandler.QuestionListResponse(this.peers, this, this.round_index, this.total_round); // send song list

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

                    waittime += Rules.TIME_WAIT_TO_PER_STEP;
                    Thread.Sleep(Rules.TIME_WAIT_TO_PER_STEP); // wait all player ready
                }
            });
        }

        public override void OnPlayerReady(HivePeer peer)
        {
            if (this.state != SongGameState.WAIT) return; // wrong request

            this.ReadyPlayerIds.Add(int.Parse(peer.UserId));
            ResponseHandler.ReadyPlayersResponse(this.peers, ReadyPlayerIds); // send message to other player

            if (this.ReadyPlayerIds.Count == this.peers.Count)
            {
                this.state = SongGameState.READY;
                this.game.ExecutionFiber.Schedule(() => this.OnNextGamePharse(), Rules.TIME_WAIT_TO_START); // change state to ready
            }
        }

        protected virtual void OnNextGamePharse()
        {
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

        public virtual void OnGameReady()
        {
            this.question_index = 0;
            this.total_question = questions.Count; // assign total question will play in current round

            this.state = SongGameState.PLAYING;
            OnNextGamePharse(); // loop to send state change message to player
        }

        public virtual void OnGamePlaying()
        {
            this.current_question = this.questions[this.question_index];

            var game_start_time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            foreach (var peer in this.peers) this.players_answer.Add(new PlayerAnswer() { question_id = this.current_question.id, user_id = int.Parse(peer.UserId), time_server = game_start_time });

            this.game.ExecutionFiber.Schedule(() =>
            {
                this.question_index++;

                this.state = SongGameState.RESULT; // set to show result state
                this.OnNextGamePharse(); // loop to send state change message to player

            }, Rules.TIME_WAIT_PER_QUESTION);
        }

        public virtual void OnGameCalculateResult()
        {
            // send result message to all user

            this.game.ExecutionFiber.Schedule(() =>
            {
                if (this.question_index >= this.total_question) this.state = SongGameState.WAIT_NEXT_ROUND; // next round 
                else this.state = SongGameState.WAIT_NEXT_QUESTION;  // next question

                this.OnNextGamePharse();
            }, Rules.TIME_WAIT_TO_SHOW_RESULT);
        }

        private void OnWaitNextPlay()
        {
            this.game.ExecutionFiber.Schedule(() =>
            {
                this.state = SongGameState.PLAYING;
                this.OnNextGamePharse();
            }, Rules.TIME_WAIT_TO_NEXT_QUESTION);
        }

        private void OnWaitNextRound()
        {
            this.round_index++;

            this.game.ExecutionFiber.Schedule(() =>
            {
                if (this.round_index >= this.total_round)
                {
                    this.state = SongGameState.END;
                }
                else {

                    this.OnAllPlayerJoined();
                }
            }, Rules.TIME_WAIT_TO_NEXT_ROUND);
        }

        public virtual void OnGameEnd()
        {
            this.game.ExecutionFiber.Schedule(this.game.Release, Rules.TIME_WAIT_TO_QUIT); // destroy game
        }

        public virtual void OnPlayerAnwserBuzz(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate the operation request 
            OperationResponse response;
            var operation = new AnwserBuzzRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            PlayerAnswer pa = players_answer.FirstOrDefault(x => x.user_id == int.Parse(peer.UserId));
            pa.time_answer = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            ResponseHandler.AnwserBuzzResponse(this.peers, int.Parse(peer.UserId), pa.time_answer - pa.time_server); // send broadcast to all player
        }

        public virtual void OnPlayerAnwserText(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate the operation request 
            OperationResponse response;
            var operation = new AnwserTextRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            PlayerAnswer pa = players_answer.FirstOrDefault(x => x.user_id == int.Parse(peer.UserId));
            if (pa.time_answer == 0) pa.time_answer = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            pa.anwser_text = operation.AnwserText;

            ResponseHandler.AnwserTextResponse(this.peers, int.Parse(peer.UserId), operation.AnwserText); // send broadcast to all player
        }

        //public virtual void AddHistory(int userId, string answer_text, int? anwser_option, int score, int time_used = 0)
        //{
        //    if (score == 0) time_used = 0;

        //    var History = new History()
        //    {
        //        game_id = this.game.Name,
        //        round_index = this.round_index,
        //        question_index = this.question_index,
        //        question_id = this.current_question.id,
        //        user_id = userId,
        //        anwser_text = answer_text,
        //        anwser_option = anwser_option,
        //        score = score,
        //        time_used = time_used
        //    };

        //    if (Histories.ContainsKey(this.round_index))
        //    {
        //        var inserted = Histories[this.round_index].Find(x => x.question_index == this.question_index);
        //        if (inserted != null)
        //            return;
        //        Histories[this.round_index].Add(History);
        //    }
        //    else {
        //        if (Histories == null) Histories = new Dictionary<int, List<History>>();
        //        Histories.Add(this.round_index, new List<History>() { History });
        //    }

        //}

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

                case MessageTag.G_ANWSER_BUZZ:
                    OnPlayerAnwserBuzz(peer, operationRequest, sendParameters);
                    break;

                case MessageTag.G_ANWSER_TEXT:
                    OnPlayerAnwserText(peer, operationRequest, sendParameters);
                    break;

                default:
                    break;
            }

        }
    }
}
#endif