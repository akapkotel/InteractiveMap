using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Location: MonoBehaviour
{
    public string locationName;
    public MapScript.House house;
    public MapScript.Owner owner;

    private Collider _collider;
    // Start is called before the first frame update

    private void Awake()
    {
        name = locationName;
    }
    void Start() {
        
    }

    public void ShowOrHideOnMap() {
        _collider = gameObject.GetComponent<Collider>();
        _collider.enabled = !_collider.enabled;
    }
}
