using UnityEngine;
using System.Collections;

namespace GuestSong
{

    public class PanelLogin : PanelBase
    {
        [SerializeField]
        private UILabel lblError = null;

        [SerializeField]
        private UIInput txtUserName;

        [SerializeField]
        private UIInput txtPassword;

        [SerializeField]
        private UIInput txtNickname;

        [SerializeField]
        private UIButton btnLogin;

        [SerializeField]
        private UIButton btnRegister;

        void Awake()
        {
            txtUserName.value = txtPassword.value = "test";
            txtNickname.value = "Test";

            EventDelegate.Add(btnLogin.onClick, onLogin);
            EventDelegate.Add(btnRegister.onClick, onLogin);
        }

        void onLogin()
        {
            NetworkManager.Instance.onCustomAuthenticationFailed -= OnCustomAuthenticationFailed;
            NetworkManager.Instance.onCustomAuthenticationFailed += OnCustomAuthenticationFailed;

            NetworkManager.Instance.Connect(txtUserName.value, txtPassword.value, false);
        }

        void onRegister()
        {
            NetworkManager.Instance.onCustomAuthenticationFailed -= OnCustomAuthenticationFailed;
            NetworkManager.Instance.onCustomAuthenticationFailed += OnCustomAuthenticationFailed;

            NetworkManager.Instance.Connect(txtUserName.value, txtPassword.value, true, txtNickname.value);
        }

        void OnCustomAuthenticationFailed(string message)
        {
            lblError.text = message;
        }
    }

}