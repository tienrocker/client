#if !UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.GameServer;
using Photon.SocketServer;

namespace Photon.LoadBalancing.Custom.Server.Interfaces
{
    public interface IGame
    {
        int Code { get; }
        string Name { get; set; }
        bool Enable { get; set; }
        void OnOperationRequest(GameClientPeer peer, int subOperationCode, OperationRequest operationRequest);
    }
}
#endif