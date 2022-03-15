using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SurvivorZombies.Zombies {
    
    public class ZombieAnimatorController : MonoBehaviour {
        
        public ZombieController zombieController;
        private Animator m_animator;
        private PhotonView m_photonView;

        [SerializeField] private GameObject m_zombieObject;

        public delegate void OnFinishAttack(GameObject zombieObject);
        public static event OnFinishAttack onFinishAttack;
        
        public delegate void OnDeath(ZombieController zombieController);
        public static event OnDeath onDeath;
        
        private void Awake() {
            m_animator = GetComponent<Animator>();
            m_photonView = GetComponentInParent<PhotonView>();
        }

        public void OnHurtAnimation() {
            if (!m_photonView.IsMine) return;
            
            m_animator.CrossFade("zombie_hurt", 0.2f);
        }

        public void OnDeathAnimation() {
            if (!m_photonView.IsMine) return;
            
            onDeath?.Invoke(zombieController);
            m_animator.CrossFade("zombie_dying", 0.2f);
        }

        public void OnAttackAnimation() {
            if (!m_photonView.IsMine) return;

            m_animator.CrossFade("zombie_attack", 0.2f);
            Invoke(nameof(OnFinishAttacking), 1.5f);
        }
        
        private void Update() {
            if (!m_photonView.IsMine) return;
            m_animator.SetFloat("deltaMovement", zombieController.Velocity.magnitude);
        }

        private void OnFinishAttacking() {
            onFinishAttack?.Invoke(m_zombieObject);
        }
    }
}