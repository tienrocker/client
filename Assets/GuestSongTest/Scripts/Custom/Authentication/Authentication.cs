#if !UNITY_5_3_OR_NEWER
using System;
using Photon.LoadBalancing.MasterServer;
using Photon.LoadBalancing.Operations;
using Photon.SocketServer;
using Photon.Common.Authentication.CustomAuthentication;
using Photon.Common.Authentication;
using Photon.LoadBalancing.Custom.Server.Data;
using Photon.LoadBalancing.Custom.Models;

namespace Photon.LoadBalancing.Custom
{
    public class Authentication
    {
        public static void CustomRegister(MasterClientPeer masterClientPeer, AuthenticateRequest authenticateRequest, SendParameters sendParameters)
        {
            try
            {
                AuthenticationData data = new AuthenticationData();
                try { data = AuthenticationData.Desserialize((byte[])authenticateRequest.ClientAuthenticationData); } catch { }
                if (String.IsNullOrEmpty(data.username)) throw new AuthenticateException("Tên đăng không được trống");
                if (String.IsNullOrEmpty(data.password)) throw new AuthenticateException("Mật khẩu không được trống");
                if (String.IsNullOrEmpty(data.nickname)) throw new AuthenticateException("Tên hiển thị không được trống");

                if (data.type == AuthenticationData.Type.DIRECT)
                {
                    ModelUser user = UserDB.GetUserByUsername(data.username);
                    if (user != null) throw new AuthenticateException("Người dùng đã tồn tại");

                    user = new ModelUser();
                    user.username = data.username;
                    user.password = data.password;

                    int i = UserDB.AddUser(user);

                    if (i > 0)
                    {
                        masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult
                        {
                            ResultCode = CustomAuthenticationResultCode.Ok,
                            Nickname = user.nickname,
                            UserId = user.id.ToString()
                        }, authenticateRequest, sendParameters, new AuthSettings());
                    }
                    else {
                        throw new AuthenticateException("Đăng ký không thành công");
                    }
                }
                else {
                    throw new AuthenticateException("Đăng ký không thành công (Không hỗ trợ)");
                }
            }
            catch (AuthenticateException ex)
            {
                masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Failed, Message = ex.Message }, authenticateRequest, sendParameters, new AuthSettings());
            }
            catch (Exception ex)
            {
                masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Failed, Message = ex.Message }, authenticateRequest, sendParameters, new AuthSettings());
            }
        }

        public static void CustomLogin(MasterClientPeer masterClientPeer, AuthenticateRequest authenticateRequest, SendParameters sendParameters)
        {
            try
            {
                AuthenticationData data = new AuthenticationData();
                try { data = AuthenticationData.Desserialize((byte[])authenticateRequest.ClientAuthenticationData); } catch { }
                if (data.register == true) { CustomRegister(masterClientPeer, authenticateRequest, sendParameters); return; }

                if (String.IsNullOrEmpty(data.username)) throw new AuthenticateException("Tên đăng không được trống");
                if (String.IsNullOrEmpty(data.password)) throw new AuthenticateException("Mật khẩu không được trống");

                if (data.type == AuthenticationData.Type.DIRECT)
                {
                    ModelUser user = UserDB.GetUserByUsername(data.username);
                    if (user == null) throw new AuthenticateException("Người dùng chưa đăng ký");

                    masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Ok, Nickname = user.nickname, UserId = user.id.ToString() }, authenticateRequest, sendParameters, new AuthSettings());
                }
                else {
                    throw new AuthenticateException("Đăng nhập không thành công (Không hỗ trợ)");
                }
            }
            catch (AuthenticateException ex)
            {
                masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Failed, Message = ex.Message }, authenticateRequest, sendParameters, new AuthSettings());
            }
            catch (Exception ex)
            {
                masterClientPeer.OnCustomAuthenticationResult(new CustomAuthenticationResult { ResultCode = CustomAuthenticationResultCode.Failed, Message = ex.Message }, authenticateRequest, sendParameters, new AuthSettings());
            }
        }

        public class AuthenticateException : Exception { public AuthenticateException(string message) : base(message) { } }
        public class AuthenticateConfirmException : Exception { public AuthenticateConfirmException(string message) : base(message) { } }
    }
}
#endif