using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorZombies.Zombies {

    public class ZombieRandomizer : MonoBehaviour {

        public List<GameObject> ZombieSkins = new List<GameObject>();
        
        private void Start() {
            var random = Random.Range(0, ZombieSkins.Count);
            ZombieSkins[random].gameObject.SetActive(true);
        }
    }
}