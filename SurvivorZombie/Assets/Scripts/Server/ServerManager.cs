using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Server {


    public class ServerManager : MonoBehaviour {
        
        [Header("input fields")]
        public TMP_InputField PlayerNameInput;
        public TMP_InputField RoomNameInputField;
        public TMP_InputField MaxPlayersInputField;
        
        public TextMeshProUGUI ConnectionStatusText;
        public TextMeshProUGUI NameWarningMessage;

        public List<GameObject> Panels;

        private readonly string connectionStatusMessage = "    Connection Status: ";
        
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
                Invoke(nameof(HideWarningMessage), 1f);
            }
            
            ActivateCurrentPanel(panelToActivate);
        }

        private void HideWarningMessage() {
            NameWarningMessage.gameObject.SetActive(false);    
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