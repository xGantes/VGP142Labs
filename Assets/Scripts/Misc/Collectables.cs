using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagerSettings;

public class Collectables : MonoBehaviour
{
    enum CollectibleType
    {
        STAR,
        HEART,
        DIAMOND,
        SPHERE,
        CRYSTAL
    }
    [SerializeField] CollectibleType collectables;
    private Rigidbody rb;
    public int gamescore;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch (collectables)
            {
                case CollectibleType.STAR:
                    GameManager.instances.score += gamescore;
                    break;
            }
            Destroy(gameObject);
        }
    }
}
