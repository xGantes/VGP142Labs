using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public Collectables[] collectableArray;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(collectableArray[Random.Range(0, 4)], transform.position, transform.rotation);
    }
}
