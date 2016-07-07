using UnityEngine;
using Photon.LoadBalancing.Custom.Models;
using Photon.LoadBalancing.Client.Data;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.LoadBalancing.Custom.Common;

namespace GuestSong
{
    public class PlayListView : MonoBehaviour
    {
        [SerializeField]
        private UIDragScrollView DragScrollView;

        [SerializeField]
        private GameObject ItemPrefab;

        [SerializeField]
        private GameObject WrapContent;
        private UIWrapContent _UIWrapContent;

        private RoomOptions roomOptions;

        public UIButton firstItem; // for auto join room

        void Awake()
        {
            DragScrollView = DragScrollView == null ? gameObject.GetComponent<UIDragScrollView>() : DragScrollView;
            ItemPrefab.GetComponent<UIDragScrollView>().scrollView = DragScrollView.scrollView;
            _UIWrapContent = WrapContent.GetComponent<UIWrapContent>();

            roomOptions = new RoomOptions();
            roomOptions.customRoomPropertiesForLobby = new string[] { Const.GameCustomProperty };
            roomOptions.maxPlayers = Rules.MAX_USER_NUMBER;
        }

        public void AddItem(ModelPlayList item)
        {
            GameObject go = GameObject.Instantiate(ItemPrefab);
            go.transform.parent = WrapContent.transform;
        }

        public bool AddedItem = false;
        public void AddItem(ModelPlayList[] items)
        {
            if (AddedItem == true) return;
            AddedItem = true;
            for (int i = 0; i < items.Length; i++)
            {
                ModelPlayList item = items[i];

                GameObject go = Instantiate(ItemPrefab);
                go.transform.parent = WrapContent.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.name = string.Format("{0} - {1}", item.id, item.name);

                UILabel label = go.GetComponentInChildren<UILabel>();
                label.text = item.name;

                EventDelegate.Add(go.GetComponent<UIButton>().onClick, () => { OnPlaylistItemClicked(item); });
                if (firstItem == null) firstItem = go.GetComponent<UIButton>();
            }

            _UIWrapContent.SortAlphabetically();
            _UIWrapContent.WrapContent();
        }

        private ModelPlayList activeItem = null;
        void OnPlaylistItemClicked(ModelPlayList item)
        {
            Debug.LogFormat("OnPlaylistItemClicked :{0}", item.id);

            activeItem = item;

            // join room with typy of id
            NetworkManager.Instance.onPhotonRandomJoinFailed -= onPhotonRandomJoinFailed;
            NetworkManager.Instance.onPhotonRandomJoinFailed += onPhotonRandomJoinFailed;

            roomOptions.customRoomProperties = new Hashtable() { { Const.GameCustomProperty, activeItem.id } };
            PhotonNetwork.JoinRandomRoom(roomOptions.customRoomProperties, roomOptions.maxPlayers);
        }

        void onPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            roomOptions.customRoomProperties = new Hashtable() { { Const.GameCustomProperty, activeItem.id } };
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
    }
}