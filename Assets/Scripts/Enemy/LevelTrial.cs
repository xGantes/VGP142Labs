using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class LevelTrial : MonoBehaviour
    {
        public Player player;

        public void OnTakeDamage(int amount)
        {
            player.currentHealth -= amount;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                OnTakeDamage(200);
                Debug.Log(player.currentHealth);
            }
        }
    }
}

