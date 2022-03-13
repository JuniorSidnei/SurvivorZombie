using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Server {

    public class PlayerLobbyEntry : MonoBehaviour  {
        [Header("UI References")]
        public TextMeshProUGUI PlayerNameText;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;
        
        private int m_ownerId;
        private bool m_isPlayerReady;
        
        public delegate void OnUpdateState();
        public static event OnUpdateState onUpdateState;
        private ExitGames.Client.Photon.Hashtable m_initialProps = new ExitGames.Client.Photon.Hashtable();    
        
        public const string PLAYER_READY = "IsPlayerReady";
        
        public void Start() {
            //if (PhotonNetwork.LocalPlayer.ActorNumber != m_ownerId) {
            //    PlayerReadyButton.gameObject.SetActive(false);
            //}
            //else {
                m_initialProps[PLAYER_READY] = m_isPlayerReady;
                PhotonNetwork.LocalPlayer.SetCustomProperties(m_initialProps);
                
                PlayerReadyButton.onClick.AddListener(() => {
                    m_isPlayerReady = !m_isPlayerReady;
                    SetPlayerReady(m_isPlayerReady);
                    
                    var props = new ExitGames.Client.Photon.Hashtable() {{PLAYER_READY, m_isPlayerReady}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient) {
                        onUpdateState?.Invoke();
                    }
                  
                });
            //}
        }
        
        public void Initialize(int playerId, string playerName) {
            m_ownerId = playerId;
            PlayerNameText.text = playerName;
        }
        
        public void SetPlayerReady(bool playerReady) {
            PlayerReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = playerReady ? "Ready!" : "Not ready";
            PlayerReadyImage.color = playerReady ? Color.green : Color.red;
        }

        public bool IsPlayerReady() {
            return m_isPlayerReady;
        }
    }
}