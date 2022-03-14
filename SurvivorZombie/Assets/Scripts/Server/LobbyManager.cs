using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using SurvivorZombies.Utils.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SurvivorZombies.Server {


    public class LobbyManager : MonoBehaviourPunCallbacks {

        enum ErrorMessageType {
            Name, RoomName, JoinRoomName
        }
        
        [Header("input fields")]
        public TMP_InputField PlayerNameInput;
        public TMP_InputField RoomNameInputField;
        public TMP_InputField MaxPlayersInputField;
        public TMP_InputField JoinRoomInputField;
        
        [Header("text fields")]
        public TextMeshProUGUI ConnectionStatusText;
        public TextMeshProUGUI NameWarningMessage;
        public TextMeshProUGUI RoomWarningMessage;
        public TextMeshProUGUI JoinRoomWarningMessage;
        public TextMeshProUGUI BestScore;

        public List<GameObject> Panels;
        public GameObject PlayerListEntry;

        public Button StartGameButton;
        public AudioClip LobbyMusic;
        public AudioClip ClickButton;
        public AudioClip ServerConnected;
        
        private Dictionary<int, PlayerLobbyEntry> m_playerListEntries = new Dictionary<int, PlayerLobbyEntry>();
        private readonly string connectionStatusMessage = "    Connection Status: ";
        private ErrorMessageType m_errorMessageType;
        private GameObject m_currentPanel;
        private bool m_isConnectedToMaster;
        private readonly string m_bestScoreName = "BestScore";
        
        public override void OnEnable() {
            base.OnEnable();
            PlayerLobbyEntry.onUpdateState += UpdatePlayersState;
        }
        
        public override void OnDisable() {
            base.OnDisable();
            PlayerLobbyEntry.onUpdateState -= UpdatePlayersState;
        }
        
        private void Start() {
            if (PlayerPrefs.HasKey(m_bestScoreName)) {
                BestScore.text = "Best current score: " + PlayerPrefs.GetInt(m_bestScoreName);
            }
            else {
                BestScore.text = "Don't have any score yet!";
            }
            AudioController.Instance.Play(LobbyMusic, AudioController.SoundType.Music, 0.2f, true);
        }

        private void Update() {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }
        
        public override void OnConnectedToMaster() {
            ActivateCurrentPanel(m_currentPanel);
        }
        
        private void UpdatePlayersState() {
            StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            //StartGameButton.gameObject.SetActive(CheckPlayersReady()); 
        }
        
        public void OnLoginButtonClicked(GameObject panelToActivate) {
            var playerName = PlayerNameInput.text;

            if (!playerName.Equals("")) {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.ConnectUsingSettings();
            }
            else {
                NameWarningMessage.gameObject.SetActive(true);
                m_errorMessageType = ErrorMessageType.Name;
                Invoke(nameof(HideWarningMessage), 1f);
            }
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.3f);
            m_currentPanel = panelToActivate;
        }

        public void OnCreateRoomButtonClicked(GameObject panelToActivate) {
            var roomName = RoomNameInputField.text;
            if (roomName == "") {
                RoomWarningMessage.gameObject.SetActive(true);
                m_errorMessageType = ErrorMessageType.RoomName;
                Invoke(nameof(HideWarningMessage), 1f);
                return;
            }

            byte.TryParse(MaxPlayersInputField.text, out var maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);
            
            var options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000, BroadcastPropsChangeToAll = true};
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.4f);
            PhotonNetwork.CreateRoom(roomName, options, null);
            ActivateCurrentPanel(panelToActivate);
        }

        public void OnJoinRoomButtonClicked(GameObject panelToActivate) {
            var roomName = JoinRoomInputField.text;
            if (roomName == "") {
                JoinRoomWarningMessage.gameObject.SetActive(true);
                m_errorMessageType = ErrorMessageType.RoomName;
                Invoke(nameof(HideWarningMessage), 1f);
                return;
            }
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.4f);
            PhotonNetwork.JoinRoom(roomName);
            ActivateCurrentPanel(panelToActivate);
        }
        
        public override void OnJoinedRoom() {
            UpdatePlayersList();
            // foreach (var player in PhotonNetwork.PlayerList) {
            //     CreatePlayerLobby(player);
            // }

            StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
            AudioController.Instance.Play(ServerConnected, AudioController.SoundType.SoundEffect2D, 0.5f);
            UpdatePlayersList();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer) {
            UpdatePlayersList();
        }
        
        public void OnStartGameButtonClicked() {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.4f);
            PhotonNetwork.LoadLevel("StreetGameScene");
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message) {
            JoinRoomWarningMessage.gameObject.SetActive(true);    
            JoinRoomWarningMessage.text = message;
            m_errorMessageType = ErrorMessageType.JoinRoomName;
            Invoke(nameof(HideWarningMessage), 1f);
        }
        
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
            if (m_playerListEntries == null) {
                m_playerListEntries = new Dictionary<int, PlayerLobbyEntry>();
            }

            if (m_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out var entry)) {
                if (changedProps.TryGetValue(PlayerLobbyEntry.PLAYER_READY, out var isPlayerReady)) {
                    entry.GetComponent<PlayerLobbyEntry>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        
        public void OnBackButtonClicked(GameObject panelToActivate) {
            if (PhotonNetwork.InLobby) {
                PhotonNetwork.LeaveLobby();
            }
            
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.4f);
            ActivateCurrentPanel(panelToActivate);
        }
        
        public void OnLeaveGameButtonClicked(GameObject panelToActivate) {
            PhotonNetwork.LeaveRoom();
            AudioController.Instance.Play(ClickButton, AudioController.SoundType.SoundEffect2D, 0.4f);
            ActivateCurrentPanel(panelToActivate);
        }
        
        public void ActivateCurrentPanel(GameObject currentPanel) {
            foreach (var panel in Panels) {
                if (panel.name == currentPanel.name) {
                    currentPanel.SetActive(true);
                    m_currentPanel = currentPanel;
                    AudioController.Instance.Play(ServerConnected, AudioController.SoundType.SoundEffect2D, 0.5f);
                }
                else {
                    panel.SetActive(false);
                }
            }
        }
        
        private bool CheckPlayersReady() {
            if (!PhotonNetwork.IsMasterClient) {
                return false; 
            }
            
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
                if (player.CustomProperties.TryGetValue(PlayerLobbyEntry.PLAYER_READY, out var isPlayerReady)) {
                    if (!(bool) isPlayerReady) {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }
            
            return true;
        }
        
        
        private  void HideWarningMessage() {
            switch (m_errorMessageType) {
                case ErrorMessageType.Name:
                    NameWarningMessage.gameObject.SetActive(false);        
                    break;
                case ErrorMessageType.RoomName:
                    RoomWarningMessage.gameObject.SetActive(false);        
                    break;
                case ErrorMessageType.JoinRoomName:
                    JoinRoomWarningMessage.gameObject.SetActive(false);        
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_errorMessageType), m_errorMessageType, null);
            }
        }

        private void CreatePlayerLobby(Photon.Realtime.Player newPlayer) {
            var entry = Instantiate(PlayerListEntry, m_currentPanel.transform, true);
            entry.transform.localScale = Vector3.one;
            var lobbyEntry = entry.GetComponent<PlayerLobbyEntry>();
            lobbyEntry.Initialize(newPlayer.ActorNumber, newPlayer.NickName);
            lobbyEntry.SetPlayerReady(false);
            m_playerListEntries.Add(newPlayer.ActorNumber, lobbyEntry); 
        }
        
        private void UpdatePlayersList() {
            foreach (var player in m_playerListEntries) {
                Destroy(player.Value.gameObject);
            }
            
            m_playerListEntries.Clear();

            if (PhotonNetwork.CurrentRoom == null) {
                return; 
            }

            foreach (var player in PhotonNetwork.CurrentRoom.Players) {
                CreatePlayerLobby(player.Value);
            }
        }
    }
}