using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Fortress : Location
{
    public MapScript.Fortresses fortressType;
    public int garrison;

    void Start()
    {
        name = locationName;
        garrison = FindObjectOfType<MapScript>().garrisonsSizes[fortressType];
    }
}
