using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace VGP142.PlayerInputs
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(CharacterController))]
    public class Enemy : MonoBehaviour
    {
        NavMeshAgent agent;
        Animator anim;
        CharacterController CC;
        public Transform playerTransform;

        [Header("Enemy Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Enemy Health")]
        public int maxHealth = 100;
        public int currentHealth;

        [Header("Enemy Attacks")]
        public Transform rangePoint;
        public Transform attackPoint;
        public LayerMask playerLayer;
        public float attackRange = 1f;
        public float attackHitbox = 1f;
        public int enemDamage = 1;
        bool playerCheck;

        //debugger
        public bool velocity;
        public bool desiredVelocity;
        public bool currentPath;


        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            CC = GetComponent<CharacterController>();
            anim = GetComponent<Animator>();
            anim.fireEvents = false;

            currentHealth = maxHealth;
        }

        private void Update()
        {
            GroundedCheck();

            agent.destination = playerTransform.position;
            anim.SetFloat("Speed", agent.velocity.magnitude);

            currentHealth = maxHealth;

            playerCheck = Physics.CheckSphere(rangePoint.position, attackRange, playerLayer);
            if (playerCheck == true)
            {
                Attack();
                anim.SetTrigger("Attack");
                SoundManager.instance.Play("Slash");
            }
        }

        private void ESEffect()
        {
            //SoundManager.instance.Play("S");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            //// update animator if using character
            //if (hasAnimator)
            //{
            //    anim.SetBool(animIDGrounded, Grounded);
            //}
        }

        private void Attack()
        {
            //Collider[] playerHit = Physics.OverlapSphere(attackPoint.position, attackHitbox, playerLayer);

            //foreach (Collider player in playerHit)
            //{
            //    Debug.Log("player hit" + enemDamage);
            //    player.GetComponent<Player>().OnTakeDamage(enemDamage);
            //}

            if (FindObjectOfType<Player>().currentHealth < 0)
            {
                Destroy(gameObject);
            }
        }

        public void OnTakeDamage(int amount)
        {
            maxHealth -= amount;

            if (maxHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

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

            if (rangePoint == null) return;
            Gizmos.DrawWireSphere(rangePoint.position, attackRange);

            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackHitbox);
            Gizmos.color = Color.cyan;

        }
    }
}
