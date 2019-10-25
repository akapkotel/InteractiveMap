using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public static Camera UserCamera { get; private set; }

    private bool LocationsVisible { get; set; }

    public Light[] ignoredLights;

    public GameObject cameraPositionIndicator;
    
    [SerializeField, Tooltip("How fast should camera move around the map?")] private float cameraMoveSpeed = 100f;

    [SerializeField, Tooltip("How fast should camera zoom in and out?")] private float cameraZoomSpeed = 100f;

    [SerializeField, Tooltip("How close mouse cursor should be placed to move camera around?")] private float mouseMoovementArea = 10f;

    [SerializeField] private float minCameraX, maxCameraX, minCameraZ, maxCameraZ;

    [SerializeField, Tooltip("How close to the ground can camera zoom down?")] private float minCameraY = 75f;

    [SerializeField,Tooltip("How high could camera fly?")] private float maxCameraY = 2000f;

    [SerializeField, Tooltip("How low camera should go to reveal locations on the map?")] 
    private float locationsHidingDistance;

    [SerializeField, Tooltip("How low camera should go to reveal locations on the map?")]
    private float smallObjectsHidingDistance;

    private GameObject[] _smallObjects;

    private bool _locationsVisible;
    private bool _smallObjectsVisible;

    private LayerMask _mask; // culling-mask used to retrieve non-culled visibility state

    private Quaternion _noRotation;
    private Vector3 _tempRotation;

    private float _cameraSpeedModifier; // modifies speed of camera according to it's height above the map

    private float _deltaTime;

    private Vector3 _cameraPosition; // used to move Camera around map

    private Transform _cameraRig;

    private const double Tolerance = 0.01;
    
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        _smallObjects = GameObject.FindGameObjectsWithTag("SmallThing");
        
        _cameraRig = transform.parent;
        _noRotation = _cameraRig.rotation;

        UserCamera = GetComponent<Camera>();
        _mask = UserCamera.cullingMask;
        
        LocationsVisible = true;
        _smallObjectsVisible = true;

        CheckLocationsVisibility();
        
        CheckSmallObjectsVisibility();
    }

    private void OnPreCull()
    {
        foreach (Light lightToIgnore in ignoredLights)
        {
            lightToIgnore.enabled = false;
        }
    }

    private void OnPostRender()
    {
        foreach (Light lightIgnored in ignoredLights)
        {
            lightIgnored.enabled = true;
        }
    }
    
    private void LateUpdate()
    {
        _deltaTime = Time.deltaTime;

        _cameraPosition = transform.parent.position;

        if (Input.GetMouseButton(1))
        {
            RotateCamera();
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            transform.parent.rotation = _noRotation;
        }
        
        _cameraSpeedModifier = _cameraPosition.y / maxCameraY;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - mouseMoovementArea) {
            _cameraPosition.z += cameraMoveSpeed * _cameraSpeedModifier * _deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= mouseMoovementArea) {
            _cameraPosition.z -= cameraMoveSpeed * _cameraSpeedModifier * _deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= mouseMoovementArea) {
            _cameraPosition.x -= cameraMoveSpeed * _cameraSpeedModifier * _deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - mouseMoovementArea)
        {
            _cameraPosition.x += cameraMoveSpeed * _cameraSpeedModifier * _deltaTime;
        }

        _cameraPosition.z = Mathf.Clamp(_cameraPosition.z, minCameraZ, maxCameraZ);
        _cameraPosition.x = Mathf.Clamp(_cameraPosition.x, minCameraX, maxCameraX);

        float scroll = Input.mouseScrollDelta.y; // Input.GetAxis("Mouse ScrollWheel");
        _cameraPosition.y -= scroll * cameraZoomSpeed * _cameraSpeedModifier * _deltaTime;
        if (Mathf.Abs(scroll) > 0)
        {
            ResizeMinimapIndicator(_cameraPosition.y);
        }

        _cameraPosition.y = Mathf.Clamp(_cameraPosition.y, minCameraY, maxCameraY);

        transform.parent.position = _cameraPosition;
        
        CheckLocationsVisibility();
        
        CheckSmallObjectsVisibility();
    }

    private void ResizeMinimapIndicator(float cameraY)
    {
        cameraPositionIndicator.transform.localScale = new Vector3(cameraY * 2, cameraY * 1.5f, 1f);
    }
    
    private void RotateCamera()
    {
        if (Math.Abs(Input.GetAxis("Mouse X")) > Tolerance || Math.Abs(Input.GetAxis("Mouse Y")) > Tolerance)
        {
            _tempRotation.x += Input.GetAxis("Mouse X") * 10f;
            _tempRotation.y -= Input.GetAxis("Mouse Y") * 10f;

            _tempRotation.y = Mathf.Clamp(_tempRotation.y, -30f, 150f);
        }

        Quaternion quaternion = Quaternion.Euler(_tempRotation.y, _tempRotation.x, 0f);

        _cameraRig.rotation = Quaternion.Lerp(transform.parent.rotation, quaternion, Time.deltaTime * 10f);
    }

    // We hide small objects and buildings when camera is high-enough:
    private void CheckLocationsVisibility()
    {
        if (transform.position.y >= locationsHidingDistance)
        {
            if (!LocationsVisible) return;
            LocationsVisible = false;
            UserCamera.cullingMask = 1;
            
            ShowOrHideLocations();
        } else {
            if (LocationsVisible) return;
            LocationsVisible = true;
            UserCamera.cullingMask = _mask;
            
            ShowOrHideLocations();
        }
    }

    private void CheckSmallObjectsVisibility()
    {
        if (transform.position.y >= smallObjectsHidingDistance) {
            if (!_smallObjectsVisible) return;
            _smallObjectsVisible = false;
            ShowOrHideSmallObjects();
        } else {
            if (_smallObjectsVisible) return;
            _smallObjectsVisible = true;
            ShowOrHideSmallObjects();
        }
    }

    private void ShowOrHideSmallObjects()
    {
        for (int i = 0; i < _smallObjects.Length; i++)
        {
            _smallObjects[i].SetActive(_smallObjectsVisible);
        }
    }

    private void ShowOrHideLocations()
    {
        for (int i = 0; i < MapScript.Instance.LocationsList.Length; i++)
        {
            GameObject location = MapScript.Instance.LocationsList[i].gameObject;
            
            location.SetActive(LocationsVisible);
        }
    }
}
