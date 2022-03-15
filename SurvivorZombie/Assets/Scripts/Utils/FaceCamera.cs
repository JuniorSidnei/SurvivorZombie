using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorZombies.Utils {
    
    public class FaceCamera : MonoBehaviour {
        private Transform m_cameraTransform;
        
        private void Start() {
            if (Camera.main != null) {
                m_cameraTransform = Camera.main.transform;    
            } 
            
        }
        
        private void Update() {
            transform.LookAt(transform.position + m_cameraTransform.rotation * Vector3.forward,
                m_cameraTransform.rotation * Vector3.up);
        }
        
    }
}