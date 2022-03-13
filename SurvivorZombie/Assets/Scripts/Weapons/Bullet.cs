using System;
using System.Collections;
using System.Collections.Generic;
using SurvivorZombies.Player.Movement;
using UnityEngine;

namespace SurvivorZombies.Weapons  {
    
    public class Bullet : MonoBehaviour {

        [SerializeField] private float m_speed;
        public LayerMask ObstacleLayer;
        private Rigidbody m_rigidbody;
        private float m_weaponDamage;
        
        private void Awake() {
            Destroy(gameObject, 3f);
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.velocity = transform.forward * m_speed;
        }

        public void SetDamage(float weaponDamage) {
            m_weaponDamage = weaponDamage;
        }
        
        private void OnTriggerEnter(Collider other)  {
            if(((1 << other.gameObject.layer) & ObstacleLayer) == 0) {
                return;
            }
            
            var damagable = other.GetComponent<IDamagable>();
            damagable?.Damage(m_weaponDamage);
            Destroy(gameObject);
        }
    }
}