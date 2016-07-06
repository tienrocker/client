#if UNITY_5_3_OR_NEWER
namespace Photon.LoadBalancing.Client.Operations.Responses
{
    using System;
    using ExitGames.Client.Photon;
    using Custom.Common;

    public class ProfileResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public int SubCode { get { return MessageTag.U_PROFILE; } }

        public ProfileResponse(OperationResponse operationResponse)
        {
            if (!operationResponse.Parameters.ContainsKey(MessageTag.KINGPLAY_OPERATION_TAG)) throw new Exception("ProfileResponse: Invalid Response Type");
            if (!operationResponse.Parameters.ContainsKey(Const.Data1)) throw new Exception("ProfileResponse: Invalid Data 1 - Id");
            if (!operationResponse.Parameters.ContainsKey(Const.Data2)) throw new Exception("ProfileResponse: Invalid Data 2 - Username");
            if (!operationResponse.Parameters.ContainsKey(Const.Data3)) throw new Exception("ProfileResponse: Invalid Data 3 - Nickname");

            try
            {
                this.Id = (int)operationResponse.Parameters[Const.Data1];
                this.Username = operationResponse.Parameters[Const.Data2] as string;
                this.Nickname = operationResponse.Parameters[Const.Data3] as string;
            }
            catch
            {
                throw new Exception("ProfileResponse: Invalid Data Type");
            }
        }
    }
}
#endif