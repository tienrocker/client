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

        public static void RequestPlayList()
        {
            new PlaylistRequest().Execute();
        }

        public static void RequestReadyPlay()
        {
            new ReadyRequest().Execute();
        }

        public static void RequestAnwserBuzz()
        {
            new AnwserBuzzRequest().Execute();
        }

        public static void RequestAnwserText(string anwserText)
        {
            new AnwserTextRequest(anwserText).Execute();
        }

        public static void RequestAnwserOption(int index)
        {
            new AnwserOptionRequest(index).Execute();
        }
    }
}
#endif