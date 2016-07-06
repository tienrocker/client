#if !UNITY_5_3_OR_NEWER
using ExitGames.Logging;
using Photon.Hive;
using Photon.LoadBalancing.Custom.Common;
using Photon.LoadBalancing.Custom.Server.Operations.Responses;
using System.Collections.Generic;
using System.Linq;

namespace Photon.LoadBalancing.Custom.Server.Logic
{
    public class GameData
    {
        #region data holder
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public static List<GameData> games = new List<GameData>();
        public static GameData GetGameData(Room room, bool init = true)
        {
            GameData logic = games.FirstOrDefault(x => x.room == room);
            if (logic == null && init)
            {
                var playListId = room.Properties.GetProperty(Const.GameCustomProperty);
                logic = new GameData() { room = room, state = Common.Quiz.GameState.NONE, PlayListId = (int)playListId.Value };
                games.Add(logic);
            }
            return logic;
        }
        public static GameData GetGameData(HivePeer peer)
        {
            GameData logic = games.FirstOrDefault(x => x.Peers.Contains(peer));
            if (logic == null) { log.Error("What the fuck are going on?"); }
            return logic;
        }
        public static bool RemoveGamedata(Room game)
        {
            GameData logic = GetGameData(game, false);
            return games.Remove(logic);
        }
        #endregion

        public List<HivePeer> Peers = new List<HivePeer>();
        public Room room { get; set; }
        public Common.Quiz.GameState state { get; set; }
        public int PlayListId { get; set; }

        public int PlayedRound { get; set; }
        public int ReadyPlayer { get; set; }
        public int[] PlayerIds { get; set; }
        public List<QuestionListResponse> QuestionList = new List<QuestionListResponse>();

    }
}
#endif