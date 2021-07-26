using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    // This is the enemy's sensitivity to noise, 0 being they cannot hear, and 1 being they hear anything higher than a specified noise floor
    [Range(0f, 1f)]
    public float hearingSensitivity = 1f;
    // public float hearingThreshold = 10f;

    // set this float if you wish to alter the hearing sensetivity temporarily. Make sure to call ResetHearing() to return the value to normal
    [Range(0f, 1f)]
    public float currentHearing;

    public bool isDebug = false;


    // Start is called before the first frame update
    void Start()
    {
        ResetHearing();
    }

    public void SetHearing(float newHearingValue)
    {
        if (newHearingValue < 0f)
        {
            currentHearing = 0f;
        }
        else if (newHearingValue > 1f)
        {
            currentHearing = 1f;
        }
        else
            currentHearing = newHearingValue;
    }

    public void ResetHearing()
    {
        currentHearing = hearingSensitivity;
    }

    public float FindHearingReduction(float intensity)
    {
        return intensity * currentHearing;
    }

    public void CanHear(float initialSoundIntensity, Vector3 playerPosition, float noiseFloor)
    {
        float intensity = initialSoundIntensity / (4 * Mathf.PI * Mathf.Pow(Vector3.Distance(playerPosition, transform.position), 2));

        // do a calculation to find the reduction of noise;
        intensity = FindHearingReduction(intensity);

        // Debug.Log(intensity);

        if (intensity > noiseFloor)
        {
            // set the target to the player's position "playerPosition"
            if (isDebug)
                Debug.Log("the Enemy can hear the Player");
        }

    }
}
