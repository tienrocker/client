#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client
{
    using Operations.Requests;

    public class RequestHandler
    {
        public static void RequestProfile(int UserId)
        {
            new ProfileRequest(UserId).Execute();
        }

        public static void RequestPlayList(int UserId)
        {
            new PlaylistRequest(UserId).Execute();
        }

        public static void RequestReadyPlay(int UserId)
        {
            new ReadyRequest(UserId).Execute();
        }
    }
}
#endif