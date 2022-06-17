using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{


    public class SpawnEnemyTrial : MonoBehaviour
    {
        public GameObject spawnNotif;

        private void OnTriggerStay(Collider other)
        {
            spawnNotif.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            spawnNotif.SetActive(false);
        }
    }
}