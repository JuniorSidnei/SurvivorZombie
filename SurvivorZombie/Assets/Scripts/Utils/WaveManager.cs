using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivorZombies.Utils
{


    public class WaveManager : MonoBehaviour {
        
        public float TimeBetweenWaves;
        public float SpawnTimer;
        public int MaxWaveNumber;
        public int MaxSpawnedObjectAtSameTime;
        public TextMeshProUGUI WaveText;
            
        private float m_spawnTimer;
        private float m_timeBetweenWaves;
        private int m_spawnedObjectsInWave = 0;
        private int m_waveNumber = 0;
        private bool m_waveCountdownFinished;
        private bool waitingToFinishWave;

        public bool CanSpawn { get; set; }

        public int ObjectsToFinishWave { get; set; }

        private void Start() {
            m_spawnTimer = SpawnTimer;
            m_timeBetweenWaves = TimeBetweenWaves;
            m_waveNumber = 0;
        }

        private void Update() {
            if (m_waveNumber > MaxWaveNumber) {
                ShowWaveText("You survived to all waves!");
                return;
            }

            if (m_waveCountdownFinished) {
                if (ObjectsToFinishWave < 0) {
                    m_waveCountdownFinished = false;
                    m_spawnedObjectsInWave = 0;
                    m_timeBetweenWaves = TimeBetweenWaves;
                    m_waveNumber++;
                    waitingToFinishWave = false;
                    ShowWaveText("Wave finished!");
                }
                
                if (waitingToFinishWave) return;
                m_spawnTimer -= Time.deltaTime;
                
                if (m_spawnTimer <= 0) {
                    CanSpawn = true;
                    m_spawnTimer = SpawnTimer;
                    m_spawnedObjectsInWave++;

                    if (m_spawnedObjectsInWave > MaxSpawnedObjectAtSameTime) {
                        waitingToFinishWave = true;
                    }
                }
            }
            else {
                m_timeBetweenWaves -= Time.deltaTime;
                if (m_timeBetweenWaves <= 0) {
                    m_waveCountdownFinished = true;
                    ShowWaveText("Wave " + m_waveNumber + " starting now! Prepare yourself!");
                    ObjectsToFinishWave = MaxSpawnedObjectAtSameTime;
                }
            }
        }

        private void ShowWaveText(string message) {
            WaveText.gameObject.SetActive(true);
            WaveText.text = message;
            Invoke(nameof(HideText), 2.5f);
        }

        private void HideText() {
            WaveText.gameObject.SetActive(false);
        }
    }
}