using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace SurvivorZombies.Weapons.Data {
    
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/Weapons", order = 1)]
    public class WeaponData : ScriptableObject {
        public GameObject Bullet;
        public int Damage;
        public float FireRate;
        public AudioClip ShootSound;
    }
}