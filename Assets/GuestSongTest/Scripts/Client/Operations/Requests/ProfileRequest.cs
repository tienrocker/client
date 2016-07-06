#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class ProfileRequest : BaseRequest
    {
        public override byte Tag { get { return MessageTag.U_PROFILE; } }

        public ProfileRequest(int UserId) : base()
        {
            this.Data.Add(Const.Data1, UserId);
        }
    }
}
#endif