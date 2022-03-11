using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorZombies.Zombies {
    
    public class ZombieAnimatorController : MonoBehaviour {
        
        private Animator m_animator;
        public TargetSeeker targetSeeker;
        
        private void Awake() {
            m_animator = GetComponent<Animator>();
            ZombieConstitution.onHurt += OnHurtAnimation;
            ZombieConstitution.onDeath += OnDeathAnimation;
        }

        private void OnHurtAnimation() {
            m_animator.CrossFade("zombie_hurt", 0.2f);
        }

        private void OnDeathAnimation() {
            m_animator.CrossFade("zombie_dying", 0.2f);
            Destroy(gameObject, 3.5f);
        }
        
        private void Update() {
            m_animator.SetFloat("deltaMovement", targetSeeker.Velocity.magnitude);
        }
    }
}