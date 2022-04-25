using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.EnemyVision
{

    [RequireComponent(typeof(EnemyVision))]
    public class EnemyStates : MonoBehaviour
    {
        private EnemyVision enemy;
        private Animator animator;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            enemy = GetComponent<EnemyVision>();
        }

        void Update()
        {
            if (animator != null && enemy.GetEnemy() != null)
            {
                animator.SetBool("Move", enemy.GetEnemy().GetMove().magnitude > 0.5f || enemy.GetEnemy().GetRotationVelocity() > 10f);
                animator.SetBool("Run", enemy.GetEnemy().IsRunning());
            }
        }
    }
}
