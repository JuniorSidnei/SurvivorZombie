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
        public GameObject EndGamePanel;

        public Image PlayerLife;
        public RoomManager RoomManager;
        public WaveManager WaveManager;
        
        private int m_bestScore;
        private int m_currentScore = 0;
        private readonly string m_bestScoreName = "BestScore";

        private void OnDisable() {
            m_bestScore = PlayerPrefs.GetInt(m_bestScoreName) < m_currentScore ? m_currentScore : PlayerPrefs.GetInt(m_bestScoreName);
            
            PlayerPrefs.SetInt(m_bestScoreName, m_bestScore);
            PlayerPrefs.Save();
        }

        private void Awake() {
            CurrentScore.text = "Score: " + m_currentScore;
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
            CurrentScore.text = "Score on play: " + m_currentScore;
            EndGamePanel.gameObject.SetActive(true);
        }

        public void OnClickRetryButton() {
            RoomManager.ReloadScene();
        }
    }
}