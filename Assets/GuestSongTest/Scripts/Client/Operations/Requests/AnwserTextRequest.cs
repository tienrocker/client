#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class AnwserTextRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.G_ANWSER_TEXT; } }

        public AnwserTextRequest(string anwserText) : base()
        {
            this.Data.Add(Const.Data1, anwserText);
        }

    }
}
#endif