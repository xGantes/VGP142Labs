using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VGP142.PlayerInputs
{
    public class GameManager : Singleton<GameManager>
    {
        [HideInInspector] public GameObject playerInstances;
        static GameManager _instances = null;
        public static GameManager instances
        {
            get
            {
                return _instances;
            }
            set
            {
                _instances = value;
            }
        }
        public GameObject playerPrefabs;

        void Start()
        {
            if (instances)
            {
                Destroy(gameObject);
            }
            else
            {
                instances = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void spawnPlayer(Transform spawnLocation)
        {
            playerInstances = Instantiate(playerPrefabs, spawnLocation.position, spawnLocation.rotation);
        }
    }
}
