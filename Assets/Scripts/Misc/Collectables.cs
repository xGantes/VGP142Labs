using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
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

        enum WeaponType
        {
            BROADSWORD
        }

        [SerializeField] CollectibleType collectables;
        [SerializeField] WeaponType weapons;
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

                switch (weapons)
                {
                    case WeaponType.BROADSWORD:
                        //do something
                        break;
                }
            }

        }
    }
}

