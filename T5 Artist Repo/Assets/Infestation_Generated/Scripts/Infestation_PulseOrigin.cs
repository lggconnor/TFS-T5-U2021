using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infestation_PulseOrigin : MonoBehaviour
{

    // Fetch the shader's emission origin variable

    public Material RefMaterial;

    void Update()
    {
        RefMaterial.SetVector("_EmissionOrigin", transform.position);
    }

}
