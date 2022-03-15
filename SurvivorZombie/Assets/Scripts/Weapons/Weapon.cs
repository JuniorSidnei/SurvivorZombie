using System;
using System.Collections;
using System.Collections.Generic;
using GameToBeNamed.Utils.Sound;
using Photon.Pun;
using SurvivorZombies.Utils.Sound;
using SurvivorZombies.Weapons;
using SurvivorZombies.Weapons.Data;
using Unity.Mathematics;
using UnityEngine;

namespace SurvivorZombies.Weapons {

    public class Weapon : MonoBehaviour {
        public WeaponData weaponData;
        public Transform barrelSpawn;
        public Transform player;
        public LayerMask aimLayer;
        public PhotonView PhotonView;
        
        
        private float m_fireRate;
        
        private void Awake() {
            m_fireRate = weaponData.FireRate;
        }

        private void Update() {
            m_fireRate -= Time.deltaTime;
            if (m_fireRate <= 0) m_fireRate = 0;
        }
        

        [PunRPC]
        public void Shoot() {
            if (m_fireRate > 0) return;
            var mouseWorldPos = Vector3.zero;
            var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            var ray = Camera.main.ScreenPointToRay(centerPoint);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayer)) {
                mouseWorldPos = hit.point;
            }

            AudioController.Instance.Play(weaponData.ShootSound, AudioController.SoundType.SoundEffect2D);
            var aimDir = (mouseWorldPos - barrelSpawn.position).normalized;
            var bullet = Instantiate(weaponData.Bullet, barrelSpawn.transform.position, quaternion.LookRotation(aimDir, Vector3.up));
            bullet.GetComponent<Bullet>().SetDamage(weaponData.Damage);
            m_fireRate = weaponData.FireRate;
        }
    }
}