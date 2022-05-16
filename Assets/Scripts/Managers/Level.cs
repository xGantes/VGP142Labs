using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class Level : MonoBehaviour
    {
        public int startingLives;
        public Transform spawnPoint;

        void Start()
        {
            GameManager.instances.spawnPlayer(spawnPoint);
        }
    }
}
