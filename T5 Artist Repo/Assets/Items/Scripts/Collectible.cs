
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Ammunition Information
    public AmmunitionCreator ammunitionCreator;

    // Properties for object movement
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public bool shouldSpin = false;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    void Start()
    {
        InitializeAmmo();
        // Store the starting position & rotation of the object
        posOffset = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        // Spin object around Y-Axis
        if(shouldSpin)
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    // Function to initialize a weapon
    void InitializeAmmo()
    {
        // Init number of bullets
        ammunitionCreator.noOfBullets = ammunitionCreator.startingBullets;
    }
}
