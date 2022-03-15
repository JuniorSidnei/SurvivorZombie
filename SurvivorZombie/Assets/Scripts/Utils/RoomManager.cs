using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivorZombies.Utils {
    
    public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable {
        
        public WaveManager WaveManager;
        private PlayerRoomManager m_playerRoomManager;
        private ZombiesRoomManager m_zombieRoomManager;
        private PhotonView m_photonView;
        
        private void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            m_photonView = GetComponent<PhotonView>();
            m_playerRoomManager = GetComponent<PlayerRoomManager>();
            m_zombieRoomManager = GetComponent<ZombiesRoomManager>();
            if (!m_photonView.IsMine || !PhotonNetwork.IsMasterClient) return;
            m_photonView.RPC(nameof(m_playerRoomManager.CreatePlayer), RpcTarget.All);
        }

        private void Update() {
            if (WaveManager.CanSpawn) {
                if (!PhotonNetwork.IsMasterClient) return;
                m_photonView.RPC(nameof(m_zombieRoomManager.CreateZombie), RpcTarget.All);
                WaveManager.CanSpawn = false;
            }
        }

        public void ReloadScene() {
            m_photonView.RPC("RPC_ReloadScene", RpcTarget.All);
        }

        public override void OnDisconnected(DisconnectCause cause) {
            SceneManager.LoadScene("SplashScreen");
        }

        public override void OnLeftRoom() {
            PhotonNetwork.Disconnect();
        }
        
        [PunRPC]
        private void RPC_ReloadScene() {
            if (!m_photonView.IsMine || !PhotonNetwork.IsMasterClient) return;
            
            PhotonNetwork.DestroyAll();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("StreetGameScene");
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(GameManager.Instance.CurrentScore.text);
            }
            else if(stream.IsReading) {
                GameManager.Instance.CurrentScore.text = (string) stream.ReceiveNext();
            }
        }
    }
}