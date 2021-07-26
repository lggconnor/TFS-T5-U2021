using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Sight))]
public class SightEditor : Editor
{
    private void OnSceneGUI()
    {
        Sight sight = (Sight)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(sight.transform.position, Vector3.up, Vector3.forward, 360, sight.sightDistance);
        float sightAngle = sight.sightConeAngle / 2;
        Vector3 viewAngA = new Vector3(Mathf.Sin((-sightAngle + sight.transform.eulerAngles.y) * Mathf.Deg2Rad), 0, Mathf.Cos((-sightAngle + sight.transform.eulerAngles.y) * Mathf.Deg2Rad));
        Vector3 viewAngB = new Vector3(Mathf.Sin((sightAngle + sight.transform.eulerAngles.y) * Mathf.Deg2Rad), 0, Mathf.Cos((sightAngle + sight.transform.eulerAngles.y) * Mathf.Deg2Rad));

        Handles.DrawLine(sight.transform.position, sight.transform.position + viewAngA * sight.sightDistance);
        Handles.DrawLine(sight.transform.position, sight.transform.position + viewAngB * sight.sightDistance);

        foreach ( Transform visibleTarget in sight.visibleTargets)
        {
            Handles.DrawLine(sight.transform.position, visibleTarget.position);
        }
    }
}
