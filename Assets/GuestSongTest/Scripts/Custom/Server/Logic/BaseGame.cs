#if !UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Custom.Server.Logic
{
    using ExitGames.Logging;
    using GameServer;
    using Photon.Hive;
    using SocketServer;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// Add flow code into Game.cs (GameServer)
    /// 
    /// public virtual SongGame logic { get; set; }
    ///
    /// protected override void PublishJoinEvent(HivePeer peer, JoinGameRequest joinRequest)
    /// {
    ///     base.PublishJoinEvent(peer, joinRequest);
    ///     if (logic == null) logic = new SongGame(peer, this);
    ///     logic.OnPlayerJoinGame(peer); // join success
    /// }
    ///
    /// protected override void PublishLeaveEvent(Actor actor, int? newMasterClientId)
    /// {
    ///     base.PublishLeaveEvent(actor, newMasterClientId);
    ///     logic.OnPlayerLeaveGame(actor.Peer); // leave success
    /// }
    /// 
    /// </summary>
    public class BaseGame
    {
        #region log
        protected static readonly ILogger log = LogManager.GetCurrentClassLogger();
        #endregion

        public BaseGame(HivePeer peer, Game game)
        {
            this.peers = new List<HivePeer>();
            this.PlayerIds = new List<int>();

            this.game = game;
        }

        public virtual List<HivePeer> peers { get; set; }
        public List<int> PlayerIds { get; set; }
        public virtual Game game { get; set; }

        public virtual void OnPlayerJoinGame(HivePeer peer)
        {
            this.peers.Add(peer);
            this.PlayerIds.Add(int.Parse(peer.UserId));
        }

        public virtual void OnPlayerLeaveGame(HivePeer peer)
        {
            this.peers.Remove(peer);
            this.PlayerIds.Remove(int.Parse(peer.UserId));
        }

        public virtual void OnPlayerReady(HivePeer peer)
        {
        }

        public virtual void OnPlayerMove(HivePeer peer)
        {

        }

        public virtual void ExecuteOperation(HivePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
        }
    }
}
#endif