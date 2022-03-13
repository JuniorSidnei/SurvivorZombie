using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks  {
        
        
        [Header("Login Panel")]
        public GameObject LoginPanel;

        [Header("Input fields")]
        public InputField PlayerNameInput;
        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        private Dictionary<string, RoomInfo> m_cachedRoomList;
        private Dictionary<string, GameObject> m_roomListEntries;
        private Dictionary<int, GameObject> m_playerListEntries;

        #region UNITY

        public void Awake() {
            PhotonNetwork.AutomaticallySyncScene = true;

            m_cachedRoomList = new Dictionary<string, RoomInfo>();
            m_roomListEntries = new Dictionary<string, GameObject>();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster() {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby() {
            // whenever this joins a new lobby, clear any previous room lists
            m_cachedRoomList.Clear();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby() {
            m_cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions {MaxPlayers = 8};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom() {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            m_cachedRoomList.Clear();
            
            SetActivePanel(InsideRoomPanel.name);

            m_playerListEntries = new Dictionary<int, GameObject>();

            foreach (Player p in PhotonNetwork.PlayerList) {
                
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);
                entry.GetComponent<PlayerListEntry>().SetPlayerReady(false);
                
                m_playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            var props = new Hashtable {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in m_playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            m_playerListEntries.Clear();
            m_playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            m_playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(m_playerListEntries[otherPlayer.ActorNumber].gameObject);
            m_playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (m_playerListEntries == null)
            {
                m_playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (m_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked() {
            string playerName = PlayerNameInput.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("DemoAsteroids-GameScene");
        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false; 
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        
        private void ClearRoomListView()
        {
            foreach (GameObject entry in m_roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            m_roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (m_cachedRoomList.ContainsKey(info.Name))
                    {
                        m_cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (m_cachedRoomList.ContainsKey(info.Name))
                {
                    m_cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    m_cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in m_cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                m_roomListEntries.Add(info.Name, entry);
            }
        }
    }
}