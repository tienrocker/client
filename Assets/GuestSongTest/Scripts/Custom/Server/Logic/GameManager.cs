#if !UNITY_5_3_OR_NEWER
using ExitGames.Logging;
using Photon.LoadBalancing.Custom.Common;
using Photon.LoadBalancing.GameServer;
using Photon.SocketServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Photon.Hive;

namespace Photon.LoadBalancing.Custom.Server.Logic
{
    public class GameManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Send game ready to all users in room
        /// </summary>
        /// <param name="data"></param>
        public static void OnAllPlayerJoined(GameData data)
        {
            if (data.state != Common.Quiz.SongGameState.NONE || data.state != Common.Quiz.SongGameState.END) { log.ErrorFormat("Game OnGameReady invalid state: {0}", data.state); }

            data.state = Common.Quiz.SongGameState.WAIT; // reset
            data.PlayedRound = 0; // reset

            ResponseHandler.SetState(data.Peers, data.state); // send game state
            ResponseHandler.QuestionList(data.Peers, data); // send song list

            data.room.ExecutionFiber.Enqueue(() => { GameManager.OnGameWait(data); }); // change state to wait
        }

        public static void OnGameWait(GameData data)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (data == null || data.Peers.Count == 0)
                    {
                        break;
                    }

                    if (data.state == Common.Quiz.SongGameState.READY)
                    {

                        break;
                    }

                    Thread.Sleep(500); // wait all player ready
                }
            });
        }

        public static void OnPlayerReady(HivePeer peer, OperationRequest operationRequest)
        {
            GameData data = GameData.GetGameData(peer);
            if (data.state != Common.Quiz.SongGameState.WAIT) return; // wrong request

            // send message to other player
            data.ReadyPlayerIds.Add(int.Parse(peer.UserId));
            ResponseHandler.ReadyList(data.Peers, data.ReadyPlayerIds);

            if (data.ReadyPlayerIds.Count == data.Peers.Count)
            {
                data.state = Common.Quiz.SongGameState.READY;
                data.room.ExecutionFiber.Enqueue(() => { GameManager.OnGameReady(data); }); // change state to ready
            }
        }

        public static void OnAllPlayerReady(GameData data)
        {


            data.room.ExecutionFiber.Enqueue(() => { GameManager.OnGameReady(data); }); // change state to ready
        }

        public static void OnGameReady(GameData data)
        {

        }

        #region init and destroy

        public static void OnPlayerJoinGame(HivePeer peer, Room room)
        {
            GameData data = GameData.GetGameData(room);
            data.Peers.Add(peer);

            data.PlayerIds = new int[data.Peers.Count];
            for (int i = 0; i < data.Peers.Count; i++) data.PlayerIds[i] = int.Parse(data.Peers[i].UserId);

            if (data.Peers.Count >= Rules.MIN_USER_NUMBER)
            {
                // lock game and another can't join game
                // room.IsVisible = false;

                // change state
                room.ExecutionFiber.Enqueue(() => { GameManager.OnAllPlayerJoined(data); });
            }
        }

        public static void OnPlayerLeaveGame(HivePeer peer, Room room)
        {
            GameData data = GameData.GetGameData(room, false);
            if (data != null)
            {
                data.Peers.Remove(peer);
                if (data.state >= Common.Quiz.SongGameState.WAIT)
                {
                    int UserId = int.Parse(peer.UserId);
                    if (data.ReadyPlayerIds.Contains(UserId))
                    {
                        data.ReadyPlayerIds.Remove(int.Parse(peer.UserId));
                    }
                }
            }
        }

        #endregion
    }
}
#endif