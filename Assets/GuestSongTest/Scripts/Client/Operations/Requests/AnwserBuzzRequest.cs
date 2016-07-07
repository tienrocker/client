#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class AnwserBuzzRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.G_ANWSER_BUZZ; } }

        public AnwserBuzzRequest() : base()
        {
        }

    }
}
#endif