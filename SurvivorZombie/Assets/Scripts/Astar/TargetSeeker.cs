using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Photon.Pun;
using SurvivorZombies.Data;
using SurvivorZombies.Player;
using SurvivorZombies.Zombies;
using UnityEngine;

public class TargetSeeker : MonoBehaviour {
    public Transform targetPosition;
    public CharacterData characterData;
    public LayerMask ObstacleLayer;
    
    private Seeker m_seeker;
    private CharacterController m_controller;
    private Path path;

    
    public float nextWaypointDistance = 3;
    public bool reachedEndOfPath;
    
    private int currentWaypoint = 0;
    private bool m_isDead;
    private Vector3 m_positionDelta;
    private PhotonView m_photonView;

    public Vector3 Velocity => m_positionDelta;

    private void OnEnable() {
        ZombieConstitution.onDeath += OnDeath;
    }

    private void OnDisable() {
        ZombieConstitution.onDeath -= OnDeath;
    }

    public void Awake () {
        m_seeker = GetComponent<Seeker>();
        m_controller = GetComponent<CharacterController>();
        m_photonView = GetComponent<PhotonView>();
    }

    private void OnDeath(GameObject zombie) {
        if (!m_photonView.IsMine) return;
        if (zombie != gameObject) return;
        
        m_isDead = true;
        Invoke(nameof(OnDestroyGameobject), 3.1f);
    }

    private void OnDestroyGameobject() {
        if (!m_photonView.IsMine) return;
        PhotonNetwork.Destroy(gameObject);
    }
    
    private void OnPathComplete (Path p) {
        if (!m_photonView.IsMine) return;
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            
            currentWaypoint = 0;
        }
    }

    public void FixedUpdate () {
        if (path == null || m_isDead || targetPosition ==  null || !m_photonView.IsMine) {
            return;
        }
        
        reachedEndOfPath = false;
        m_seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        float distanceToWaypoint;
        while (true) {
           
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
               
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }

        var oldPos = transform.position;
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;
        var dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        var velocity = dir * characterData.Speed * speedFactor;
        m_controller.SimpleMove(velocity);
        m_positionDelta = transform.position - oldPos;
    }

    public void SetTarget(Transform target) {
        if (!m_photonView.IsMine) return;
        targetPosition = target;
        m_seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }
    
    private void OnTriggerEnter(Collider other) {
        if (!m_photonView.IsMine) return;
        
        if(((1 << other.gameObject.layer) & ObstacleLayer) == 0) {
            return;
        }
        
        if (targetPosition) {
            return;
        }

        targetPosition = other.transform;
        m_seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }
}
