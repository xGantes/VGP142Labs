using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VGP142.PlayerInputs
{
    public class LookAtPlayer : MonoBehaviour
    {
        public Transform cam;

        private void LateUpdate()
        {
            transform.LookAt(cam);
        }
    }
}
