using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject[] pickupPrefab;

    private void Start()
    {
        int randomNum = Random.Range(0, pickupPrefab.Length);

        Instantiate(pickupPrefab[randomNum], transform.position, transform.rotation);
    }

}
