using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameManagerSettings
{
    public class GameManager : Singleton<GameManager>
    {
        static GameManager _instances = null;

        [HideInInspector] public GameObject playerInstances;
        [HideInInspector] public UnityEvent<int> onLifeEvent;
        [HideInInspector] public UnityEvent<int> onScoreEvent;
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

        int _score = 0;
        int _lives = 1;
        public int maxlives = 3;
        public GameObject playerPrefabs;

        public int score
        {
            get { return _score; }
            set
            {
                _score = value;
                onScoreEvent.Invoke(value);
                Debug.Log("Score set to:" + score.ToString());
            }
        }
        public int lives
        {
            get { return _lives; }
            set
            {
                if (_lives > value)
                {
                    Destroy(playerInstances);
                    //spawnPlayer(currentLevel.spawnPoint);
                }

                _lives = value;
                if (_lives > maxlives)
                {
                    _lives = maxlives;
                    onLifeEvent.Invoke(value);
                }
                Debug.Log("Lives set to:" + lives.ToString());
            }

        }

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
