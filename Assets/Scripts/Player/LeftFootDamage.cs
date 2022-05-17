using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFootDamage : MonoBehaviour
{
    public int lhdamage = 40;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Left Foot Hit " + lhdamage);
            other.GetComponent<EnemyHealth>().OnTakeDamage(lhdamage);
        }
    }
}
