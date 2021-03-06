using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class HealingBonfire : MonoBehaviour
    {
        private GameManager gameManager;
        public Transform spawnPoint;
        public GameObject healingEffect;

        public Player playerHealth;
        public float healthInterval = 4f;
        public int healthGain = 2; 
        private float healthIntervalLeft = 0f;
        private bool isHealing = false;

        private void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        void FixedUpdate()
        {

            healthIntervalLeft -= Time.fixedDeltaTime;
            if (healthIntervalLeft <= 0f)
            {
                isHealing = true;
                healthIntervalLeft += healthInterval;
            }
            else
            {
                isHealing = false;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (isHealing)
            {
                if (playerHealth.currentHealth != 100)
                {
                    playerHealth.currentHealth += healthGain;
                    healingEffect.SetActive(true);
                    //SoundManager.PlaySound("healing");
                }
                else
                {
                    isHealing = false;
                    healingEffect.SetActive(false);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            isHealing = false;
            healingEffect.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Debug.Log("CheckPoint");
                gameManager.lastCheckPoint = spawnPoint.transform.position;
            }
        }
    }
}

