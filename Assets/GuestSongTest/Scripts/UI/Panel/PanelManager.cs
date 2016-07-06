using UnityEngine;
using System.Collections;
namespace GuestSong
{
    public class PanelManager : MonoBehaviour
    {

        public static PanelManager Instance;

        public GameObject PanelLogin;
        public GameObject PanelLoading;
        public GameObject PanelDisconnected;
        public GameObject PanelLobby;
        public GameObject PanelGame;

        public PanelType ActivePanel = PanelType.NONE;
        public enum PanelType { NONE, LOADING, DISCONNECTED, LOGIN, LOBBY, GAME, }

        void Awake()
        {
            Instance = this;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 25;
            Application.runInBackground = true;
        }

        public void Show(PanelType type)
        {
            if (type == ActivePanel) return;

            ActivePanel = type;
            Hide(ActivePanel);

            switch (ActivePanel)
            {
                // GAME LOGIC
                case PanelType.LOGIN:
                    PanelLogin.SetActive(true);
                    PanelLogin.GetComponent<PanelLogin>().OnShow();
                    break;

                case PanelType.LOBBY:
                    PanelLobby.SetActive(true);
                    PanelLobby.GetComponent<PanelLobby>().OnShow();
                    break;

                case PanelType.GAME:
                    PanelGame.SetActive(true);
                    PanelGame.GetComponent<PanelGame>().OnShow();
                    break;

                // EXTRA PANEL
                case PanelType.LOADING:
                    PanelLoading.SetActive(true);
                    break;

                case PanelType.DISCONNECTED:
                    PanelDisconnected.SetActive(true);
                    break;

                default:
                    break;
            }
        }

        public void Hide(PanelType other)
        {
            if (other != PanelType.LOGIN && PanelLogin.activeSelf == true)
            {
                PanelLogin.GetComponent<PanelLogin>().OnHide();
                PanelLogin.SetActive(false);
            }

            if (other != PanelType.LOBBY && PanelLobby.activeSelf == true)
            {
                PanelLobby.GetComponent<PanelLobby>().OnHide();
                PanelLobby.SetActive(false);
            }

            if (other != PanelType.GAME && PanelGame.activeSelf == true)
            {
                PanelGame.GetComponent<PanelGame>().OnHide();
                PanelGame.SetActive(false);
            }

            if (other != PanelType.LOADING)
            {
                PanelLoading.SetActive(false);
            }

            if (other != PanelType.DISCONNECTED)
            {
                PanelDisconnected.SetActive(false);
            }
        }
    }
}