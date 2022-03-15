using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SurvivorZombies.Utils
{


    public class WaveManager : MonoBehaviourPun {
        
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
        private bool m_isCharacterDead;
        private int m_waveMessageIndex;
        public bool CanSpawn { get; set; }

        public int ObjectsToFinishWave { get; set; }

        private Dictionary<int, string> m_waveMessages = new Dictionary<int, string>();
        
        private void OnEnable() {
            PlayerConstitution.onCharacterDeath += OnCharacterDeath;
        }

        private void OnDisable() {
            PlayerConstitution.onCharacterDeath += OnCharacterDeath;
        }

        private void OnCharacterDeath(GameObject playerObject) {
            m_isCharacterDead = true;
        }
        
        private void Start() {
            m_spawnTimer = SpawnTimer;
            m_timeBetweenWaves = TimeBetweenWaves;
            m_waveNumber = 0;
            m_waveMessages.Add(0, "Wave " + m_waveNumber + " starting now! Prepare yourself!");
            m_waveMessages.Add(1, "Wave finished!");
            m_waveMessages.Add(2, "You survived to all waves!");
        }

        private void Update() {
            if (m_isCharacterDead) return;
            
            if (m_waveNumber > MaxWaveNumber) {
                m_waveMessageIndex = 2;
                photonView.RPC("ShowWaveText", RpcTarget.All);
                Invoke(nameof(ReturnToLobby), 2f);
                return;
            }

            if (m_waveCountdownFinished) {
                if (ObjectsToFinishWave <= 0) {
                    m_waveCountdownFinished = false;
                    m_spawnedObjectsInWave = 0;
                    m_timeBetweenWaves = TimeBetweenWaves;
                    m_waveNumber++;
                    waitingToFinishWave = false;
                    m_waveMessageIndex = 1;
                    photonView.RPC("ShowWaveText", RpcTarget.All);
                }
                
                if (waitingToFinishWave) return;
                m_spawnTimer -= Time.deltaTime;
                
                if (m_spawnTimer <= 0) {
                    CanSpawn = true;
                    m_spawnTimer = SpawnTimer;
                    m_spawnedObjectsInWave++;

                    if (m_spawnedObjectsInWave >= MaxSpawnedObjectAtSameTime) {
                        waitingToFinishWave = true;
                    }
                }
            }
            else {
                m_timeBetweenWaves -= Time.deltaTime;
                if (m_timeBetweenWaves <= 0) {
                    m_waveCountdownFinished = true;
                    photonView.RPC("ShowWaveText", RpcTarget.All);
                    m_waveMessageIndex = 0;
                    ObjectsToFinishWave = MaxSpawnedObjectAtSameTime;
                }
            }
        }

        [PunRPC]
        private void ShowWaveText() {
            WaveText.gameObject.SetActive(true);
            WaveText.text = m_waveMessages[m_waveMessageIndex];
            Invoke(nameof(HideText), 2.5f);
        }
        
        private void HideText() {
            WaveText.gameObject.SetActive(false);
        }

        private void ReturnToLobby() {
            PhotonNetwork.LeaveRoom();
        }
    }
}