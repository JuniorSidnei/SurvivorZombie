using System;
using System.Collections;
using System.Collections.Generic;
using SurvivorZombies.Player.Movement;
using UnityEngine;

namespace SurvivorZombies.Weapons.Bullet  {
    
    public class Bullet : MonoBehaviour {

        [SerializeField] private float m_speed;
        private Rigidbody m_rigidbody;
        
        private void Awake()
        {
            Destroy(gameObject, 3f);
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.velocity = transform.forward * m_speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }
    }
}