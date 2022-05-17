using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandDamage : MonoBehaviour
{
    public int rhdamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Right Hand Hit " + rhdamage);
            other.GetComponent<EnemyHealth>().OnTakeDamage(rhdamage);
        }
    }
}
