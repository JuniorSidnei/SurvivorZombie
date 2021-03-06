using Photon.Pun;
using SurvivorZombies.Data;
using SurvivorZombies.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivorZombies.Player.Movement  {

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour {

        [SerializeField] private Weapon m_currentWeapon;
        public CharacterData CharacterData;
        private CharacterController m_characterController;
        private PlayerInput m_playerInput;
        private Vector3 m_playerVelocity;
        private Vector3 m_velocityDelta;
        private bool m_isGrounded;
        private bool m_isShooting;
        private bool m_isDead;
        private Transform m_cameraTranform;
        private PhotonView m_photonView;
        
        private float m_jumpHeight = 1f;
        private float m_gravityValue = -9.81f;

        private InputAction m_moveAction;
        private InputAction m_jumpAction;
        private InputAction m_shootAction;

        public Vector3 VelocityDelta => m_velocityDelta;

        public bool IsGrounded => m_isGrounded;

        public delegate void OnPlayerJump(GameObject playerObject);
        public static event OnPlayerJump onJump;
        
        private void Awake()  {
            m_characterController = GetComponent<CharacterController>();
            m_playerInput = GetComponent<PlayerInput>();
            
            if (Camera.main != null) {
                m_cameraTranform = Camera.main.transform;
            }

            m_moveAction = m_playerInput.actions["Move"];
            m_jumpAction = m_playerInput.actions["Jump"];    
            m_shootAction = m_playerInput.actions["Shoot"];

            m_photonView = GetComponent<PhotonView>();
        }

        private void OnEnable() {
            if (!m_photonView.IsMine) return;
            m_shootAction.started += _ => Shoot(true);
            m_shootAction.canceled += _ => Shoot(false);
            PlayerConstitution.onCharacterDeath += OnCharacterDeath;
        }

        private void OnDisable()  {
            if (!m_photonView.IsMine) return;
            m_shootAction.performed -= _ => Shoot(false);
            PlayerConstitution.onCharacterDeath -= OnCharacterDeath;
        }

        private void Shoot(bool isShooting) {
            if (!m_photonView.IsMine) return;
            m_isShooting = isShooting; 
        }

        private void OnCharacterDeath(GameObject playerObject) {
            m_isDead = true;
        }
        
        private void FixedUpdate() {
            if (!m_photonView.IsMine) return;

            if (m_isDead) return;
            
            if (m_isShooting) {
                m_currentWeapon.Shoot();
            }

            m_isGrounded = m_characterController.isGrounded;
            if (m_isGrounded && m_playerVelocity.y < 0) {
                m_playerVelocity.y = 0f;
            }

            var oldPos = transform.position;
            var move = new Vector3(m_moveAction.ReadValue<Vector2>().x, 0, m_moveAction.ReadValue<Vector2>().y);
            move = move.x * m_cameraTranform.right.normalized + move.z * m_cameraTranform.forward.normalized;
            move.y = 0;
            m_characterController.Move(move * Time.deltaTime * CharacterData.Speed);

            if (m_jumpAction.triggered && m_isGrounded) {
                onJump?.Invoke(gameObject);
                m_playerVelocity.y += Mathf.Sqrt(m_jumpHeight * -3.0f * m_gravityValue);
            }

            m_playerVelocity.y += m_gravityValue * Time.deltaTime;
            m_characterController.Move(m_playerVelocity * Time.deltaTime);
            m_velocityDelta = transform.position - oldPos;

            var rotation = Quaternion.Euler(0, m_cameraTranform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 6f);
        }
    }
}