#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class ReadyRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.G_READY; } }

        public ReadyRequest() : base()
        {
        }

    }
}
#endif