using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace SurvivorZombies.Utils {
    
    public class GameManager : Singleton<GameManager> {
        
        public TextMeshProUGUI BestTextScore;
        public TextMeshProUGUI CurrentScore;
        
        private PhotonView m_photonView;
        private int m_bestScore;
        private int m_currentScore = 0;
        private readonly string m_bestScoreName = "BestScore";

        private void OnDisable() {
            PlayerPrefs.SetInt(m_bestScoreName, m_currentScore);
        }

        private void Awake() {
            if (PlayerPrefs.HasKey(m_bestScoreName)) {
                m_bestScore = PlayerPrefs.GetInt(m_bestScoreName);
            }
            else {
                m_bestScore = 0;
            }

            BestTextScore.text = "Best Score: " + m_bestScore;
            CurrentScore.text = "Current Score: " + m_currentScore;
        }
    
        public void UpdateGlobalScore() {
            m_currentScore ++;
            CurrentScore.text = "Current Score: " + m_currentScore;
        }
    }
}