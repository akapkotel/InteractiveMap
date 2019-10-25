using UnityEngine;

public class Fortress : Location
{
    public MapScript.Fortresses fortressType;
    public int garrison;

    private void Awake()
    {
        locationType = MapScript.LocationType.Forteca;
        garrison = MapScript.GarrisonsSizes[fortressType];
    }
}
