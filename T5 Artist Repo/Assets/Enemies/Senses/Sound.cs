using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Sound : MonoBehaviour
{
    float soundRadius;

    public float soundFloor = 0.1f;

    [Range(0f, 100f)]    public float initialSoundIntensity = 10f;

    // Start is called before the first frame update
    void Start()
    {
        soundRadius = 0f;
        GetComponent<SphereCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        soundRadius = Mathf.Sqrt(initialSoundIntensity / (4 * Mathf.PI * soundFloor));

        GetComponent<SphereCollider>().radius = soundRadius;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Hearing>().CanHear(initialSoundIntensity, transform.position, soundFloor);
        }
    }
}
