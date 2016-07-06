#if UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Common;
using System.Collections.Generic;

namespace Photon.LoadBalancing.Client.Operations.Requests
{
    public class BaseRequest
    {
        public virtual byte Code { get { return MessageTag.KINGPLAY_OPERATION_CODE; } }
        public virtual byte Tag { get { return 0; } }
        public virtual Dictionary<byte, object> Data { get; set; }

        public BaseRequest()
        {
            this.Data = new Dictionary<byte, object>();
            this.Data.Add(MessageTag.KINGPLAY_OPERATION_TAG, this.Tag);
        }

        public void Execute()
        {
            PhotonNetwork.networkingPeer.OpCustom(this.Code, this.Data, true);
        }
    }
}
#endif