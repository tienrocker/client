#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class ReadyRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.G_READY; } }

        public ReadyRequest(int UserId) : base()
        {
            this.Data.Add(Const.Data1, UserId);
        }

    }
}
#endif