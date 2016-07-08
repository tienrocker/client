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

        [SerializeField]
        private bool autoLogin = false;
        private int autoLoginTimes = 0;
        [SerializeField]
        private int maxAutoLoginTimes = 10;

        void Awake()
        {
            txtUserName.value = txtPassword.value = "test";
            txtNickname.value = "Test";

            EventDelegate.Add(btnLogin.onClick, onLogin);
            EventDelegate.Add(btnRegister.onClick, onLogin);
            EventDelegate.Add(txtUserName.onSubmit, onLogin);
        }

        public override void OnShow()
        {
            base.OnShow();
            NetworkManager.Instance.onCustomAuthenticationFailed += OnCustomAuthenticationFailed;

            this.txtUserName.isSelected = true;
            if (!PhotonHandler.AppQuits && this.autoLogin && this.autoLoginTimes < this.maxAutoLoginTimes)
            {
                NetworkManager.Instance.Connect(txtUserName.value, txtPassword.value, false);
                this.autoLoginTimes++;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            NetworkManager.Instance.onCustomAuthenticationFailed -= OnCustomAuthenticationFailed;
        }

        void onLogin()
        {
            NetworkManager.Instance.Connect(txtUserName.value, txtPassword.value, false);
        }

        void onRegister()
        {
            NetworkManager.Instance.Connect(txtUserName.value, txtPassword.value, true, txtNickname.value);
        }

        void OnCustomAuthenticationFailed(string message)
        {
            lblError.text = message;
        }
    }

}