using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class EnemyDamage : MonoBehaviour
    {
        public int rhdamage = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //Debug.Log("Right Hand Hit " + rhdamage);
                other.GetComponent<Player>().OnTakeDamage(rhdamage);
            }
        }
    }
}

