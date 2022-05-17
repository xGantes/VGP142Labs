using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.EnemyVision
{
    [RequireComponent(typeof(EnemyVision))]
    public class EnemyStates : MonoBehaviour
    {
        public GameObject exclama_prefab;
        public GameObject death_fx_prefab;

        private EnemyVision enemy;
        private Animator animator;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            enemy = GetComponent<EnemyVision>();
            enemy.onAlert += OnAlert;
        }

        void Update()
        {
            if (animator != null && enemy.GetEnemy() != null)
            {
                animator.SetBool("Move", enemy.GetEnemy().GetMove().magnitude > 0.5f || enemy.GetEnemy().GetRotationVelocity() > 10f);
                animator.SetBool("Run", enemy.GetEnemy().IsRunning());
            }
        }

        private void OnAlert(Vector3 target)
        {
            if (exclama_prefab != null)
                Instantiate(exclama_prefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            if (animator != null)
                animator.SetTrigger("Surprised");
        }

        private void OnDeath()
        {
            if (death_fx_prefab)
                Instantiate(death_fx_prefab, transform.position + Vector3.up * 0.5f, death_fx_prefab.transform.rotation);
        }
    }
}
