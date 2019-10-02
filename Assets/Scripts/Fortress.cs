using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Fortress : Location
{
    public MapScript.Fortresses fortressType;
    public int garrison;

    // Start is called before the first frame update
    void Start()
    {
        this.name = locationName;
        garrison = FindObjectOfType<MapScript>().garrisonsSizes[fortressType];
    }
}
