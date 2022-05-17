using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;

    public void OnTakeDamage(int amount)
    {
        maxHealth -= amount;
        if (maxHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
