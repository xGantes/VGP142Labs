using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class HealingBonfire : MonoBehaviour
    {
        public Player playerHealth;
        public float healthInterval = 1f;
        public int healthGain = 2; 
        private float healthIntervalLeft = 0f;
        private bool isHealing = false;

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
                playerHealth.currentHealth += healthGain;
            }
        }
    }
}

