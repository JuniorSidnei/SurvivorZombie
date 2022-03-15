using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace SurvivorZombies.Utils {
    
    public class SetCharacterName : MonoBehaviourPun, IPunObservable {
        public TextMeshProUGUI NameText;
        
        private void Start() {
            if (!photonView.IsMine) return;

            NameText.text = photonView.Owner.NickName;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(NameText.text);
            }
            else {
                NameText.text = (string) stream.ReceiveNext();
            }
        }
    }
}