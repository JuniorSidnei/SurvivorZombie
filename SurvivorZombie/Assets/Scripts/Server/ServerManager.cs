using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Server {


    public class ServerManager : MonoBehaviour {

        enum ErrorMessageType {
            Name, RoomName
        }
        
        [Header("input fields")]
        public TMP_InputField PlayerNameInput;
        public TMP_InputField RoomNameInputField;
        public TMP_InputField MaxPlayersInputField;
        
        public TextMeshProUGUI ConnectionStatusText;
        public TextMeshProUGUI NameWarningMessage;
        public TextMeshProUGUI RoomWarningMessage;

        public List<GameObject> Panels;

        private readonly string connectionStatusMessage = "    Connection Status: ";
        private ErrorMessageType m_errorMessageType;
        
        private void Update() {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }
        
        public void OnLoginButtonClicked(GameObject panelToActivate) {
            var playerName = PlayerNameInput.text;

            if (!playerName.Equals("")) {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else {
                NameWarningMessage.gameObject.SetActive(true);
                m_errorMessageType = ErrorMessageType.Name;
                Invoke(nameof(HideWarningMessage), 1f);
            }
            
            ActivateCurrentPanel(panelToActivate);
        }

        public void OnCreateRoomButtonClicked() {
            var roomName = RoomNameInputField.text;
            if (roomName == "") {
                RoomWarningMessage.gameObject.SetActive(true);
                m_errorMessageType = ErrorMessageType.RoomName;
                Invoke(nameof(HideWarningMessage), 1f);
                return;
            }

            byte.TryParse(MaxPlayersInputField.text, out var maxPlayers);
            maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);
            
            var options = new RoomOptions {MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
            //TODO implement joined room and instantiate players text in the room
        }
        
        public void OnCancelCreateRoomPanel(GameObject panelToActivate) {
            ActivateCurrentPanel(panelToActivate);
        }
        
        private  void HideWarningMessage() {
            switch (m_errorMessageType) {
                case ErrorMessageType.Name:
                    NameWarningMessage.gameObject.SetActive(false);        
                    break;
                case ErrorMessageType.RoomName:
                    RoomWarningMessage.gameObject.SetActive(false);        
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_errorMessageType), m_errorMessageType, null);
            }
        }

        private void ActivateCurrentPanel(GameObject currentPanel) {
            foreach (var panel in Panels) {
                if (panel.name == currentPanel.name) {
                    currentPanel.SetActive(true);
                }
                else {
                    panel.SetActive(false);
                }
            }
        }
    }
}