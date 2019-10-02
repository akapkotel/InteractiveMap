using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{

    public Light[] ignoredLights;

    void OnPreCull()
    {
        foreach (Light light in ignoredLights)
        {
            light.enabled = false;
        }
    }

    void OnPostRender()
    {
        foreach (Light light in ignoredLights)
        {
            light.enabled = true;
        }
    }
}
