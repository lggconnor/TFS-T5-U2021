using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public Transform[] entrances;

    public Transform[] exits;

    // center can be rough, but MUST be further from the entrance than from the exit if not exact.
    public Vector3 center = Vector3.zero;

    // the dimensions should be a little smaller than the actual size of the level
    public Vector3 roughDimensions = new Vector3(0.9f, 0.9f, 0.9f);

    public bool isEntrance = false;

    [HideInInspector] public Transform spawnPoint;
    [HideInInspector] public GameObject playerReference;

    // Start is called before the first frame update
    void Start()
    {
        if (isEntrance && playerReference && spawnPoint)
        {
            Instantiate(playerReference, spawnPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
