#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class AnwserOptionRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.G_ANWSER_OPTION; } }

        public AnwserOptionRequest(int index) : base()
        {
            this.Data.Add(Const.Data1, index);
        }
    }
}
#endif