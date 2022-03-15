using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace SurvivorZombies.Utils {
    
    public class SetCharacterName : MonoBehaviourPun {
        public TextMeshProUGUI NameText;
        
        private void Start() {
            if (!photonView.IsMine) return;

            NameText.text = photonView.Owner.NickName;
        }

        
    }
}