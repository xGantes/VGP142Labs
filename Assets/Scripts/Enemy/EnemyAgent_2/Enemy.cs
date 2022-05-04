using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    public Transform playerTransform;

    //debugger
    public bool velocity;
    public bool desiredVelocity;
    public bool currentPath;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.fireEvents = false;
    }

    private void Update()
    {
        agent.destination = playerTransform.position;
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void OnDrawGizmos()
    {
        if (velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }
        if (desiredVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }
        if (currentPath)
        {
            Gizmos.color = Color.black;
            var agentPath = agent.path;
            Vector3 prevCorners = transform.position;
            foreach (var currentCorner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorners, currentCorner);
                Gizmos.DrawSphere(currentCorner, 0.1f);

                prevCorners = currentCorner;
            }
        }
    }
}
