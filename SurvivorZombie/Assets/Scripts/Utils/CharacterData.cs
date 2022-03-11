using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorZombies.Data {

    [CreateAssetMenu(menuName = "ScriptableObjects/Data/Character", order = 2)]
    public class CharacterData : ScriptableObject {
        public float MaxHealth;
        public float Speed;
        public float Damage;
    }
}