using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFootCollider : MonoBehaviour
{
    public int leftFootDamage = 40;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Player Damage " + leftFootDamage);
            other.GetComponent<EnemyHealth>().takeDamage(leftFootDamage);
        }
    }
}
