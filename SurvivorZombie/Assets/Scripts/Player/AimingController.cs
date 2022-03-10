using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurvivorZombies.Player  {

    public class AimingController : MonoBehaviour {
        public Image crossHairAiming;
        public Image crossHair;
        [SerializeField] private PlayerInput m_playerInput;
        private CinemachineVirtualCamera m_virtualAimCamera;

        private InputAction m_aimAction;

        private int m_priorityBoost = 15;

        private void Awake() {
            m_virtualAimCamera = GetComponent<CinemachineVirtualCamera>();
            m_aimAction = m_playerInput.actions["Aim"];
            
        }

        private void OnEnable() {
            m_aimAction.performed += _ => Aiming(true);
            m_aimAction.canceled += _ => Aiming(false);
        }

        private void OnDisable() {
            m_aimAction.performed -= _ => Aiming(false);
            m_aimAction.canceled -= _ => Aiming(false);
        }

        private void Aiming(bool isAiming)  {
            if (isAiming) {
                m_virtualAimCamera.Priority += m_priorityBoost;
            }
            else {
                m_virtualAimCamera.Priority -= m_priorityBoost;
            }
            crossHair.gameObject.SetActive(!isAiming);
            crossHairAiming.gameObject.SetActive(isAiming);
        }
    }
}