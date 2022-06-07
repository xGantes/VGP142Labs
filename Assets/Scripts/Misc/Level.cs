using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class Level : MonoBehaviour
    {
        public int startingLives;
        public Transform spawnPoint;
        // Start is called before the first frame update
        void Start()
        {
            //GameManager.instances.lives = startingLives;
            //GameManager.instances.score = 0;
            //GameManager.instances.spawnPlayer(spawnPoint);
            //GameManager.instances.currentLevel = this;
        }
    }
}

