using System;
using System.Collections;
using System.Collections.Generic;
using SurvivorZombies.Weapons;
using SurvivorZombies.Weapons.Data;
using UnityEngine;

namespace SurvivorZombies.Weapons {

    public class Weapon : MonoBehaviour {
        public WeaponData WeaponData;
        public Transform BarrelSpawn;
        
        private float m_fireRate;
        
        private void Awake() {
            m_fireRate = WeaponData.FireRate;
        }

        private void Update() {
            m_fireRate -= Time.deltaTime;
            if (m_fireRate <= 0) m_fireRate = 0;
        }

        public void Shoot() {
            if (m_fireRate > 0) return; 
            Instantiate(WeaponData.Bullet, BarrelSpawn.transform.position, Quaternion.identity);
            m_fireRate = WeaponData.FireRate;
        }
    }
}