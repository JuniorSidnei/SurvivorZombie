using System;
using System.Collections;
using System.Collections.Generic;
using SurvivorZombies.Player.Movement;
using UnityEngine;

namespace SurvivorZombies.Player.Animations {
    
    public class PlayerAnimatorController : MonoBehaviour {
        
        [SerializeField] private PlayerController m_playerController;
        private Animator m_animator;
        
        private void Awake() {
            m_animator = GetComponent<Animator>();
            PlayerController.onJump += OnPlayerJump;
        }

        private void OnPlayerJump() {
            m_animator.CrossFade("pistol_jump", 0.2f);
        }
        
        private void FixedUpdate() {
            m_animator.SetFloat("deltaX", m_playerController.VelocityDelta.normalized.x);
            
            m_animator.SetFloat("deltaZ", m_playerController.VelocityDelta.normalized.z);
        }
    }
}