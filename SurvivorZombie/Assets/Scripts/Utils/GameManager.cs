using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Utils {
    
    public class GameManager : Singleton<GameManager> {
        
        public TextMeshProUGUI BestTextScore;
        public TextMeshProUGUI CurrentScore;
        public TextMeshProUGUI FinishedScore;
        public GameObject EndGamePanel;
        public GameObject RetryButton;

        public Image PlayerLife;
        public RoomManager RoomManager;
        public WaveManager WaveManager;

        private PhotonView m_photonView;
        private int m_bestScore;
        private int m_currentScore = 0;
        private readonly string m_bestScoreName = "BestScore";

        private void OnDisable() {
            m_bestScore = PlayerPrefs.GetInt(m_bestScoreName) < m_currentScore ? m_currentScore : PlayerPrefs.GetInt(m_bestScoreName);
            
            PlayerPrefs.SetInt(m_bestScoreName, m_bestScore);
            PlayerPrefs.Save();
        }

        private void Awake() {
            CurrentScore.text = "Current Score: " + m_currentScore;
            m_photonView = GetComponent<PhotonView>();
        }
    
        public void UpdateScore() {
            m_currentScore ++;
            CurrentScore.text = "Current Score: " + m_currentScore;
            WaveManager.ObjectsToFinishWave--;
        }

        public void UpdateLife(float currentFill) {
            PlayerLife.DOFillAmount(currentFill, 0.2f);
        }

        public void EndGame() {
            if (PlayerPrefs.HasKey(m_bestScoreName)) {
                BestTextScore.text = "Best Score: " + PlayerPrefs.GetInt(m_bestScoreName);
            }
            else {
                BestTextScore.text = "No best score yet!";
            }
            FinishedScore.text = "Score on play: " + m_currentScore;
            EndGamePanel.gameObject.SetActive(true); 
            RetryButton.SetActive(PhotonNetwork.IsMasterClient);
        }
        
        public void OnClickRetryButton() {
            EndGamePanel.gameObject.SetActive(false);
            RoomManager.ReloadScene();
        }
    }
}