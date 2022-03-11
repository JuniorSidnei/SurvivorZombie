using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using SurvivorZombies.Data;
using SurvivorZombies.Zombies;
using UnityEngine;

public class TargetSeeker : MonoBehaviour {
    public Transform targetPosition;
    public CharacterData characterData;
    
    private Seeker m_seeker;
    private CharacterController m_controller;
    private Path path;

    
    public float nextWaypointDistance = 3;
    public bool reachedEndOfPath;
    
    private int currentWaypoint = 0;
    private bool m_isDead;

    public Vector3 Velocity => m_controller.velocity;
    
    public void Start () {
        m_seeker = GetComponent<Seeker>();
        m_controller = GetComponent<CharacterController>();
        
        m_seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        ZombieConstitution.onDeath += OnDeath;
    }

    private void OnDeath() {
        m_isDead = true;
        Destroy(gameObject, 4.1f);
    }
    
    private void OnPathComplete (Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            
            currentWaypoint = 0;
        }
    }

    public void FixedUpdate () {
        if (path == null || m_isDead) {
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
         
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;
        var dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        var velocity = dir * characterData.Speed * speedFactor;
        m_controller.SimpleMove(velocity);
    }
}
