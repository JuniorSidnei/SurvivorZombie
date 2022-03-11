using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class TargetSeeker : MonoBehaviour {
     public Transform targetPosition;

    private Seeker m_seeker;
    private CharacterController m_controller;

    private Path path;

    public float speed = 2;

    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public bool reachedEndOfPath;

    public void Start () {
        m_seeker = GetComponent<Seeker>();
        m_controller = GetComponent<CharacterController>();
        
        m_seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
    }

    public void OnPathComplete (Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            
            currentWaypoint = 0;
        }
    }

    public void FixedUpdate () {
        if (path == null) {
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
        var velocity = dir * speed * speedFactor;
        m_controller.SimpleMove(velocity);
    }
}
