using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandCollider : MonoBehaviour
{
    public int rightHandDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Player Damage " + rightHandDamage);
            other.GetComponent<EnemyHealth>().takeDamage(rightHandDamage);
        }
    }
}
