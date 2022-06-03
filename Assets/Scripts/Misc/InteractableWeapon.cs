using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class InteractableWeapon : MonoBehaviour
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}

