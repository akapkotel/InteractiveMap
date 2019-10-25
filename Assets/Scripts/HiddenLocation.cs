using UnityEngine;

public class HiddenLocation : Location
{
    public string locationPassword;

    private bool _availableToFind;

    private void Awake()
    {
        known = UserProfile.Instance.FoundHiddenLocations.Contains(locationName);

        _availableToFind = locationPassword.Length == 0 || UserProfile.Instance.UserPasswords.Contains(locationPassword);
    }

    protected override TextMesh CreateLocationMapLabel()
    {
        TextMesh locationMapLabel =  base.CreateLocationMapLabel();
        
        // hide Location name on the map from the User, he must point this Location with mouse to "find" it
        locationMapLabel.gameObject.SetActive(known); // he could already found location last time he used map!
        return locationMapLabel;
    }

    protected override void HighlightLocationObjects(bool highlight)
    {
        if (!_availableToFind) return;
        
        base.HighlightLocationObjects(highlight);
    
        // start displaying name of Location since it is now known to the User
        LocationMapLabel.gameObject.SetActive(true);

        if (known) return;
        
        RevealLocation();
    }
    
    private void RevealLocation()
    {
        if (!UserProfile.Instance.FoundHiddenLocations.Contains(locationName)) {
            UserProfile.Instance.FoundHiddenLocations.Add(locationName);
        }

        known = true;
        MapScript.Instance.LocationNames.Add(locationName);
    }

    protected override void OnMouseDown()
    {
        if (!_availableToFind) return;
        
        base.OnMouseDown();
    }
}

