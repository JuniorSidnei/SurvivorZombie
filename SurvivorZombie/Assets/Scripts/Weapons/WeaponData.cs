using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorZombies.Weapons.Data {
    
    [CreateAssetMenu(menuName = "ScriptableObjects/WeaponsData", order = 1)]
    public class WeaponData : ScriptableObject {
        public GameObject Bullet;
        public int Damage;
        public float FireRate;
    }
}