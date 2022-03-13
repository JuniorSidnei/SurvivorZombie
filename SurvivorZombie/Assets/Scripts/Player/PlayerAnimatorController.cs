using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using SurvivorZombies.Player.Movement;
using UnityEngine;

namespace SurvivorZombies.Player.Animations {
    
    public class PlayerAnimatorController : MonoBehaviour {
        
        [SerializeField] private PlayerController m_playerController;
        private Animator m_animator;
        private PhotonView m_photonView;
        [SerializeField] private GameObject m_playerObject;
          
        private void Awake() {
            m_animator = GetComponent<Animator>();
            m_photonView = GetComponentInParent<PhotonView>();
            PlayerController.onJump += OnPlayerJump;
        }

        private void OnPlayerJump(GameObject playerObject) {
            if (!m_photonView.IsMine) return;
            if (m_playerObject != playerObject) return;
            
            m_animator.CrossFade("pistol_jump", 0.2f);
        }
        
        private void FixedUpdate() {
            if (!m_photonView.IsMine) return;
            m_animator.SetFloat("deltaX", m_playerController.VelocityDelta.normalized.x);
            m_animator.SetFloat("deltaZ", m_playerController.VelocityDelta.normalized.z);
        }
    }
}